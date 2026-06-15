# BrightestSwagShop Selenium Automation Tests

## Overview

This project contains automated end-to-end tests for the BrightestSwagShop application using Selenium WebDriver, NUnit and ExtentReports.

The purpose of this project is to automatically enable and disable bug toggles through the admin dashboard and verify their impact on the application from an end-user perspective.

---

## Technologies

* C#
* .NET
* Selenium WebDriver
* NUnit
* ChromeDriver
* ExtentReports
* MongoDB
* Docker

---

## Project Structure

### BaseTest

Responsible for:

* Browser setup
* Browser cleanup
* Test initialization
* Admin login helper methods
* Report initialization

### LoginPage

Responsible for:

* Microsoft authentication
* Email input
* Password input
* Pick an account handling
* Stay signed in handling

### UserLoginPage

Responsible for:

* Standard user login

### AdminDashboardPage

Responsible for:

* Navigation inside the admin dashboard
* Opening the Bugs page

### TogglePage

Responsible for:

* Reading toggle status
* Enabling/disabling bug toggles
* Managing bug configurations

### ProductPage

Responsible for:

* Product page navigation
* Product validation
* Add To Cart validation

### ReportManager

Responsible for:

* HTML report generation
* Test result logging
* Report storage

---

## Supported Bug Toggles

The following bug toggles can be tested automatically:

* Wrong Cart Total
* Disable Add To Cart
* Login Fails
* Slow Loading
* Product API Error
* Broken Favorites
* Broken Images

---

## Test Workflow

### 1. Login as Administrator

Selenium performs Microsoft authentication using an administrator account.

### 2. Open Admin Bugs Page

After successful login, Selenium navigates to:

```text
http://localhost:5173/admin/bugs
```

### 3. Enable or Disable Bug Toggle

The selected bug toggle is activated or deactivated.

### 4. Login as Standard User

A standard user logs into the webshop.

### 5. Validate Bug Impact

Selenium verifies whether the bug is visible in the application.

Example:

```text
Disable Add To Cart
↓
User opens product page
↓
Add To Cart button is disabled
↓
Test passes
```

---

## Requirements

The following services must be running before executing the tests:

### Frontend

```bash
npm run dev
```

### Backend API

```bash
dotnet run
```

### MongoDB

MongoDB Docker container must be running.

---

## Running Tests

Run all tests:

```bash
dotnet test
```

Run a specific test:

```bash
dotnet test --filter "LoginFails_Can_Be_Toggled"
```

List all available tests:

```bash
dotnet test --list-tests
```

---

## Test Reporting

After execution, an HTML report is automatically generated using ExtentReports.

The report contains:

* Test name
* Pass/Fail status
* Execution time
* Error messages

Generated file:

```text
TestReport.html
```

---

## Objective

The goal of this project is to automate regression testing within BrightestSwagShop, allowing bugs to be detected faster and reducing manual testing effort.
