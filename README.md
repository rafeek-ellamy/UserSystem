Project Name UserSystem

Overview

This project is built using .NET 8 on the backend with the Repository Pattern and Angular 18 on the frontend.

It implements role-based authorization, ensuring that users can only access the parts of the application permitted by their assigned roles.

----------------------

Technologies Used:

Backend: .NET 8, Entity Framework Core (with Repository Pattern), JWT Authentication (Authorization via user claims)

----------------------

Link = https://github.com/rafeek-ellamy/UserSystem-UI

Frontend:
Angular 18,
PrimeNG,
PrimeFlex,
User Roles and Authorization,
The system supports three primary user roles:

----------------------

("UserName":"SuperAdmin", "Password":"P@ssw0rd")

Super Admin: Has full access, including managing users and updating roles.

Admin: Limited administrative privileges (configurable by the Super Admin).

System User: Default role assigned upon registration, with basic access rights.

----------------------

Role Management

When a user registers, they are assigned the System User role by default.

A Super Admin (predefined in the database) can update, add, or delete users and manage their roles.

Authorization is enforced via claims stored in the JWT token.

The frontend routes are safeguarded based on user roles, preventing unauthorized access.

----------------------


Features

✅ User authentication & authorization (JWT-based).

✅ Role-based access control (RBAC).

✅ Super Admin can manage users and roles.

✅ Secure API endpoints protected by user claims.

✅ Angular routes safeguarded based on roles.
