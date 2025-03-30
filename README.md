# Project Description  
This is a lead management system developed as a full-stack technical challenge, using:  
* **Backend**: .NET Core Web API with SQL Server  
* **Frontend**: Angular (SPA)  

## Key Features  
####  Tab "Invited" (New Leads)  
* Lead listing with basic information  
* Available actions:  
  - **Accept**:  
    - Updates status to "Accepted"  
    - Applies 10% discount if price > $500  
    - Simulates email sending to sales@test.com  
  - **Decline**: Updates status to "Declined"  

####  Tab "Accepted" (Accepted Leads)  
* Listing of accepted leads with full details  

## Technologies Used  
### Backend  
* .NET Core 6  
* Entity Framework Core  
* SQL Server  
* RESTful API  

### Frontend  
* Angular  
* Reusable components  
* API consumption services  

## How to Run  
### Prerequisites  
* .NET 6 SDK  
* SQL Server  
* Node.js  

## Notes  
* Email service is simulated (does not send real emails)  
* Icons used: Font Awesome  
* The project demonstrates clean architecture patterns and best practices 
