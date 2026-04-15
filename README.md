# User Management API – Coursera Project

This project is a basic User Management API built using .NET 10 and ASP.NET Core for a Coursera assignment.

## Features

### 1. CRUD Operations

The API supports all basic operations to manage users:

* **GET /api/users** → Get all users
* **GET /api/users/{id}** → Get a user by ID
* **POST /api/users** → Add a new user
* **PUT /api/users/{id}** → Update an existing user
* **DELETE /api/users/{id}** → Remove a user

---

### 2. Middleware

* **Logging Middleware**
  Logs each request and response, including method, URL, status code, and execution time.

* **Authentication Middleware**
  Uses a simple API key system. All `/api/*` routes require a valid `X-Api-Key` header.

---

### 3. Data Validation

* Uses data annotations to check:

  * Name length
  * Email format
  * Allowed roles
* Ensures each email is unique when creating or updating users.

---

### 4. AI Assistance

Github Copilot (Antigravity) was used to:

* Generate basic code structure
* Help write business logic in the UserService
* Build logging and authentication middleware
* Fix errors and ensure the project runs correctly

---

## Running the Project

1. Install .NET 10 SDK
2. Clone the repository
3. Go to the project folder
4. Run:

```bash
dotnet run
```

The API will run at:
`https://localhost:5001` (or another port set in `launchSettings.json`)

---

## Authentication

To use the API, include this header in your requests:

* **Header:** `X-Api-Key`
* **Value:** your API key (set in `appsettings.json`)
