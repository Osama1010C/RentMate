
# 🏡 Property Rental Platform

Welcome to the **Property Rental Platform** – a full-stack solution designed to seamlessly connect **Landlords** 🧑‍💼 and **Tenants** 👨‍💻, with **Admins** 🛡️ managing the ecosystem. From listing properties to real-time chat and rental proposals, this system streamlines property rentals with ease and efficiency.

### 👥 Collaborator

- 🧑‍💻 **GitHub**: <a href="https://github.com/Islam25x" target="_blank" rel="noopener noreferrer">Islam Salah</a> — Frontend developer  
- 📂 **Frontend Project Repository**: <a href="https://github.com/Islam25x/homeless" target="_blank" rel="noopener noreferrer">Homless Repo</a> — Source code.


## 🚀 Live Demo — Try it Now!
Experience the platform in action:  
- 🌍 **Frontend**: <a href="https://homeless-lovat.vercel.app/" target="_blank" rel="noopener noreferrer">Homeless</a> — Sleek and responsive user interface for tenants and landlords.  
- ⚙️ **API Swagger**: <a href="http://rentmate.runasp.net/swagger" target="_blank" rel="noopener noreferrer">RentMate API</a> — Fully documented and testable REST API.

> 💡 Explore available properties, register as a landlord, or dive into the API right from your browser!

## 🌟 Features

### 👥 Actors
- 🛡️ **Admin**: Oversees the platform, verifies users, and moderates content.
- 🧑‍💼 **Landlord**: Manages property listings and handles rental applications.
- 👨‍💻 **Tenant**: Explores properties, submits proposals, and communicates in real time.

### 🔐 Authentication & Authorization
- Tenants can **browse properties without logging in**, but need to log in to apply or message.
- Admins **approve or reject** landlord registrations and property listings.
- JWT Authentication integrated with Swagger for secure and easy testing.

### 🏠 Property Listings
- Includes 📛 Landlord Name, 🏷️ Title, 📝 Description, 💰 Price, 📍 Location, 🖼️ Images, 👁️ View Count, and 📌 Rental Status.
- Advanced search: filter by **location**, **price**, and more.
- Realtime status updates using **SignalR**.

### 📄 Rental Proposals
- Tenants can **apply for rentals** with documents.
- Save favorite listings 💾.
- Landlords can **review, accept, or reject** proposals.
- A property becomes **unavailable** once rented.

### 💬 Real-Time Communication
- Built-in **chat system using SignalR** for direct messaging between tenants and landlords.
- Comment system for property discussions.

---

## 🧠 Architecture Overview

```
📁 Controllers         --> API endpoints
📁 DTOModels           --> Request/Response models
📁 Data
    ├── Models         --> Entity models
    └── DbContext      --> Database access (Scaffolded using Reverse Engineering)
📁 Extensions          --> JWT + Service registration helpers
📁 Exceptions          --> Exception Classes
📁 Hub                 --> SignalR hub for real-time chat 💬
📁 Repositories        --> Generic Repository pattern for data access
📁 Services            --> Business logic layer
📁 Helpers             --> Helper Methods to serve some services
📁 Validation          --> check validations of models, images, and files
📁 Middleware
📁 Logs                --> trace execute time for each endpoint
📁 UOF (Unit of Work)  --> Transaction management layer
```

---

## 🛠️ Technologies Used

### 🔧 Backend Technologies

- ⚙️ **.NET Core** (Web API)
- 🛠️ **Entity Framework Core** with Scaffolded Reverse Engineering
- 🔄 **SignalR** for real-time features
- 🔐 **JWT** for secure authentication
- 📘 **Swagger** for API documentation and testing
- 🗄️ **SQL Server** for data persistence

### 🚀 Frontend Technologies

- ⚛️ React + TypeScript
- 🧠 Redux Toolkit + RTK Query
- 🎨 Bootstrap (Responsive UI)
- 🧪 React Testing Library
- 🖼️ Image Upload + Preview (Base64)
- 🌐 Routing via React Router

---

## 🗂️ Database Schema

Here is the database schema illustrating the relationships between the entities:

<img src="Data/DB%20ERD%20Schema.png" alt="Database Schema" width="1000"/>




---


