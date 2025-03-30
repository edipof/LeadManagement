using LeadManagement.Domain.Entities;
using LeadManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace LeadManagement.Infrastructure.Tests.Repositories
{
    public class LeadRepositoryTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Lead>> _mockLeadsDbSet;
        private readonly LeadRepository _repository;
        private readonly List<Lead> _sampleLeads;

        public LeadRepositoryTests()
        {
            _sampleLeads = new List<Lead>
            {
                new("John", "Doe", "Sydney", "Plumbing", "Fix leaking pipe", 400m, "john@example.com", "0412345678", 1001)
                {
                    Id = 1,
                    DateCreated = DateTime.UtcNow.AddDays(-2),
                    Status = "Invited"
                },
                new("Jane", "Smith", "Melbourne", "Electrical", "Install lights", 600m, "jane@example.com", "0423456789", 1002)
                {
                    Id = 2,
                    DateCreated = DateTime.UtcNow.AddDays(-1),
                    Status = "Invited"
                },
                new("Bob", "Brown", "Brisbane", "Carpentry", "Build deck", 300m, "bob@example.com", "0434567890", 1003)
                {
                    Id = 3,
                    DateCreated = DateTime.UtcNow,
                    Status = "Accepted"
                },
                new("Alice", "Johnson", "Perth", "Painting", "Paint house", 500m, "alice@example.com", "0445678901", null)
                {
                    Id = 4,
                    DateCreated = DateTime.UtcNow.AddHours(-12),
                    Status = "Declined"
                }
            };

            // Configuração avançada do DbSet mockado
            _mockLeadsDbSet = new Mock<DbSet<Lead>>();

            // Configuração para operações síncronas
            _mockLeadsDbSet.As<IQueryable<Lead>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Lead>(_sampleLeads.AsQueryable().Provider));

            _mockLeadsDbSet.As<IQueryable<Lead>>()
                .Setup(m => m.Expression)
                .Returns(_sampleLeads.AsQueryable().Expression);

            _mockLeadsDbSet.As<IQueryable<Lead>>()
                .Setup(m => m.ElementType)
                .Returns(_sampleLeads.AsQueryable().ElementType);

            _mockLeadsDbSet.As<IQueryable<Lead>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => _sampleLeads.GetEnumerator());

            // Configuração para operações assíncronas
            _mockLeadsDbSet.As<IAsyncEnumerable<Lead>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Lead>(_sampleLeads.GetEnumerator()));

            // Configuração do FindAsync
            _mockLeadsDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => _sampleLeads.FirstOrDefault(l => l.Id == (int)ids[0]));

            // Configuração do contexto
            _mockContext = new Mock<IApplicationDbContext>();
            _mockContext.Setup(c => c.Leads).Returns(_mockLeadsDbSet.Object);

            _repository = new LeadRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetLeadByIdAsync_ShouldReturnCorrectLeadWithAllProperties()
        {
            // Arrange
            int expectedId = 2;

            // Act
            Lead result = await _repository.GetLeadByIdAsync(expectedId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedId, result.Id);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Smith", result.LastName);
            Assert.Equal("Melbourne", result.Suburb);
            Assert.Equal("Electrical", result.Category);
            Assert.Equal("Install lights", result.Description);
            Assert.Equal(600m, result.Price);
            Assert.Equal("jane@example.com", result.Email);
            Assert.Equal("0423456789", result.PhoneNumber);
            Assert.Equal(1002, result.JobId);
            Assert.Equal("Invited", result.Status);
            Assert.True(result.DateCreated < DateTime.UtcNow);
        }

        [Fact]
        public async Task GetLeadByIdAsync_ShouldReturnNullForNonExistentLead()
        {
            // Arrange
            int nonExistentId = 99;

            // Act
            Lead result = await _repository.GetLeadByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLeadsByStatusAsync_ShouldReturnOnlyLeadsWithMatchingStatus()
        {
            // Arrange
            string status = "Invited";
            int expectedCount = 2;

            // Act
            List<Lead> result = await _repository.GetLeadsByStatusAsync(status);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, lead => Assert.Equal(status, lead.Status));
        }

        [Fact]
        public async Task GetLeadsByStatusAsync_ShouldReturnEmptyListForNonExistentStatus()
        {
            // Arrange
            string nonExistentStatus = "Completed";

            // Act
            List<Lead> result = await _repository.GetLeadsByStatusAsync(nonExistentStatus);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateLeadAsync_ShouldUpdateDateUpdated()
        {
            // Arrange
            DateTime initialDate = DateTime.UtcNow.AddDays(-1);
            Lead lead = new("Test", "Lead", "Sydney", "Plumbing", "Desc", 100m,
                               "test@test.com", "123456789", 1)
            {
                Id = 1,
                DateCreated = initialDate,
                DateUpdated = initialDate
            };

            _mockLeadsDbSet.Setup(m => m.FindAsync(1)).ReturnsAsync(lead);

            // Captura o lead atualizado
            Lead updatedLead = null;
            _mockLeadsDbSet.Setup(m => m.Update(It.IsAny<Lead>()))
                          .Callback<Lead>(l => updatedLead = l);

            // Act
            await _repository.UpdateLeadAsync(lead);

            // Assert
            Assert.NotNull(updatedLead);
            Assert.NotEqual(initialDate, updatedLead.DateUpdated);
            Assert.Equal(initialDate, updatedLead.DateCreated);

            // Verifica se a data atualizada está dentro de um intervalo razoável
            Assert.InRange(updatedLead.DateUpdated,
                         DateTime.UtcNow.AddSeconds(-10),
                         DateTime.UtcNow.AddSeconds(1));

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateLeadAsync_ShouldThrowExceptionWhenDbContextFails()
        {
            // Arrange
            Lead leadToUpdate = _sampleLeads.First();
            _mockContext.Setup(m => m.SaveChangesAsync(default))
                .ThrowsAsync(new DbUpdateException("Database update failed"));

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _repository.UpdateLeadAsync(leadToUpdate));
        }

        [Fact]
        public async Task UpdateLeadAsync_ShouldApplyDiscountWhenAcceptingLead()
        {
            // Arrange
            Lead leadToUpdate = _sampleLeads.First(l => l.Id == 2); // Lead com preço 600
            leadToUpdate.AcceptLead();

            // Act
            await _repository.UpdateLeadAsync(leadToUpdate);

            // Assert
            _mockLeadsDbSet.Verify(m => m.Update(It.Is<Lead>(l =>
                l.Id == 2 &&
                l.Status == "Accepted" &&
                l.Price == 540m // 600 - 10%
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateLeadAsync_ShouldHandleNullJobId()
        {
            // Arrange
            Lead leadWithNullJobId = new("Test", "Lead", "Suburb", "Category", "Description", 100m,
                                            "test@test.com", "123456789", null)
            {
                Id = 5,
                Status = "Invited", // DEVE estar como "Invited" para poder ser aceito
                DateCreated = DateTime.UtcNow
            };

            _sampleLeads.Add(leadWithNullJobId); // Adiciona à lista de dados de teste

            // Configurar o mock para retornar nosso lead
            _mockLeadsDbSet.Setup(m => m.FindAsync(5))
                          .ReturnsAsync(leadWithNullJobId);

            // Act
            leadWithNullJobId.AcceptLead(); // Primeiro muda o status
            await _repository.UpdateLeadAsync(leadWithNullJobId); // Depois atualiza

            // Assert
            _mockLeadsDbSet.Verify(m => m.Update(It.Is<Lead>(l =>
                l.Id == 5 &&
                l.JobId == null &&
                l.Status == "Accepted" // Verifica se o status foi atualizado
            )), Times.Once);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            Type expectedResultType = typeof(TResult).GetGenericArguments()[0];
            object? executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }
}