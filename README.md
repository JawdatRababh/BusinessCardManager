![image](https://github.com/user-attachments/assets/84323cf5-90f5-4af6-940d-b0a3d6275342)## Overview
BusinessCardManager is a web-based application for managing business cards, allowing users to store, filter, and export business card information. It supports importing and exporting data in CSV and XML formats.

## Features
- Create, update, and delete business card information.
- Import business card data from CSV and XML.
- Export business card data to CSV and XML.
- Search and filter business card details by various fields.
- Supports pagination and sorting.
- View buiness card Template after create

## Technologies Used
- **Backend**: .NET Core Web API (C#)
- **Frontend**: Angular
- **Database**: MS SQL Server
- **Testing**: xUnit, Moq
- **Other Libraries**: Angular Material, PrimeNG

## Setup Instructions

### Prerequisites
- .NET SDK (8.0)
- Node.js (14.x or higher)
- Angular CLA(13.0.0.0)
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

Backend: https://localhost:44372 if it is run as IIS Express
Frontend: http://localhost:4200 (ng s) in cmd
Testing
To run the backend unit tests:

bash
Copy code
cd BusinessCardApi.Tests
dotnet test

Some steps to run the project from VS.

1) Make sure the API is a startup project
2) Run the Api as IISExpress
3) Open cmd and write  F:\Business Card Manager\BusinessCard\Web\business-card-app (Just change the directory on whatever you cloned th project) then ng s
![image](https://github.com/user-attachments/assets/8e6ab4b2-220a-4d8e-8786-1fd665006562)

![image](https://github.com/user-attachments/assets/36956af8-e8f6-412c-81dc-a0f8828970c7)




Some Images from system 


![image](https://github.com/user-attachments/assets/a5aa9ccd-43dd-490a-a33c-49a1da19221a)

![image](https://github.com/user-attachments/assets/a0b773cb-1614-44b9-bc03-8187b014b2f3)

![image](https://github.com/user-attachments/assets/536b5c8a-199f-4170-83f7-959d0fcbdefa)

![image](https://github.com/user-attachments/assets/09a95d45-d875-41ae-9ba2-813ca0c4fd4a)

![image](https://github.com/user-attachments/assets/c49853c4-1e2d-4dcb-bf51-6eb0c2117a8d)

![image](https://github.com/user-attachments/assets/8a26769c-2bf1-4fa3-9eef-a31f2793fd81)

![image](https://github.com/user-attachments/assets/c1530e16-0659-4aa7-895a-34930c371ccc)



