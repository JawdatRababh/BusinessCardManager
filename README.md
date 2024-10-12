## Overview
BusinessCardManager is a web-based application for managing business cards, allowing users to store, filter, and export business card information. It supports importing and exporting data in CSV and XML formats.

## Features
- Create, update, and delete business card information.
- Import business card data from CSV and XML.
- Export business card data to CSV and XML.
- Search and filter business card details by various fields.
- Supports pagination and sorting.

## Technologies Used
- **Backend**: .NET Core Web API (C#)
- **Frontend**: Angular
- **Database**: MS SQL Server
- **Testing**: xUnit, Moq
- **Other Libraries**: Angular Material, PrimeNG

## Setup Instructions

### Prerequisites
- .NET SDK (6.0+)
- Node.js (14.x or higher)
- SQL Server (LocalDB or full version)

### Steps to Run the Application Locally
1. Clone the repository:
   ```bash
   git clone https://github.com/JawdatRababh/BusinessCardManager.git
Navigate to the backend project folder:

bash
Copy code
cd BusinessCard
Restore .NET dependencies:

bash
Copy code
dotnet restore
Run the backend API:

bash
Copy code
dotnet run
Navigate to the frontend project folder:

bash
Copy code
cd Web/business-card-app
Install frontend dependencies:

bash
Copy code
npm install
Run the Angular frontend:

bash
Copy code
ng serve
The application should now be running on:

Backend: https://localhost:5001
Frontend: http://localhost:4200
Testing
To run the backend unit tests:

bash
Copy code
cd BusinessCardApi.Tests
dotnet test