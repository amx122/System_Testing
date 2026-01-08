# SmartTest System

**SmartTest** is a comprehensive desktop application designed for automated knowledge assessment and educational process management. Built on the **.NET** platform using **WPF**, the system offers a robust solution for creating, distributing, and analyzing educational tests. It features a secure, multi-role environment that streamlines the interaction between students, teachers, and administrators.

---

## 📖 Overview

The primary objective of SmartTest is to digitize the testing workflow, eliminating manual grading and ensuring objective evaluation. The application leverages a **multi-tier architecture**, separating the presentation layer (UI) from business logic and data access, ensuring scalability and maintainability.

The system supports complex testing scenarios, including time limits, multimedia questions, and instant feedback with detailed error analysis.

---

## 🚀 Key Features

### 🎓 For Students
* **Intuitive Dashboard:** Real-time visualization of academic progress, total attempts, and average scores.
* **Interactive Testing:** Distraction-free testing interface with a countdown timer.
* **Instant Results:** Automated grading immediately after test completion.
* **Detailed Error Analysis:** A specialized review mode allowing students to inspect their attempts, highlighting correct (green) and incorrect (red) answers.
* **History Log:** Comprehensive journal of all past attempts with dates and scores.

### 👨‍🏫 For Teachers
* **Test Constructor:** Flexible editor for creating tests with custom titles, descriptions, and time limits (5–120 minutes).
* **Rich Content:** Ability to attach images to questions for better visual context.
* **Question Types:** Support for single-choice (Radio Buttons) and multiple-choice (Checkboxes) questions.
* **Content Management:** Edit or delete existing tests via a context menu.

### 🛡️ For Administrators
* **User Management:** Centralized panel to register new teachers/admins and remove obsolete accounts.
* **Security:** Role-based access control and protection against critical data deletion.
* **Search & Export:** Quick search functionality and data export capabilities (JSON reports).

---

## 🛠️ Technology Stack

The project relies on a modern Microsoft technology stack to ensure performance and reliability:

* **Language:** C#
* **Framework:** .NET 6.0 / .NET 8.0
* **UI Framework:** Windows Presentation Foundation (WPF)
* **Design System:** Material Design in XAML Toolkit (Google Material Design principles)
* **Database:** SQLite (Embedded relational database)
* **ORM:** Entity Framework Core (Code-First approach)
* **Security:** SHA-256 Hashing for password storage

---

## 📸 Interface & Screenshots

### 1. Dashboard & Navigation
*The central hub providing quick access to tests and statistics.*

<img width="1079" height="707" alt="image" src="https://github.com/user-attachments/assets/d9889268-772c-4be8-befd-a9d3015e6de3" />


### 2. Test Execution
*The focused testing environment with a countdown timer and multimedia support.*

<img width="880" height="587" alt="image" src="https://github.com/user-attachments/assets/14c773fb-69bf-4d3f-8a83-aeae966240f9" />



### 3. Detailed Error Analysis
*The review window that allows students to analyze their mistakes.*

<img width="783" height="588" alt="image" src="https://github.com/user-attachments/assets/4d4ca2c6-07be-4470-b469-7483447f3eb6" />


---

## ⚙️ Installation & Setup

To set up the project locally, follow these steps:

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/your-username/smarttest-system.git](https://github.com/your-username/smarttest-system.git)
    ```

2.  **Open the project:**
    Launch the `SmartTest.sln` file in **Visual Studio 2022**.

3.  **Restore Dependencies:**
    Visual Studio should automatically restore NuGet packages. If not, run:
    ```bash
    dotnet restore
    ```

4.  **Database Initialization:**
    The application uses **EF Core Code-First**. You do **not** need to create the database manually.
    * On the first launch, the application will automatically generate the `kursovva.db` SQLite file.
    * A default Administrator account will be created automatically.

5.  **Run the Application:**
    Press `F5` or click "Start" in Visual Studio.

---

## 🔐 Default Credentials

Upon the first launch, use the following credentials to access the system as an Administrator:

* **Login:** `admin`
* **Password:** `admin`

*Note: It is recommended to create a new admin user and delete the default one for security purposes in a production environment.*

---

## 🏗️ Architecture Details

The solution is structured to ensure modularity:

* **Data Layer:** Manages `AppDbContext` and migrations using Entity Framework Core.
* **Models:** Defines POCO classes (`User`, `Exam`, `Question`, `TestResult`, `UserAnswer`) with Data Annotations.
* **Logic/UI:** WPF Windows handle the interaction logic, utilizing event-driven programming and data binding to update the UI dynamically.

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
