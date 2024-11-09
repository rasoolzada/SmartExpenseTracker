# SmartExpenseTracker

**SmartExpenseTracker** is an ASP.NET Core MVC web application designed to help users manage and track their expenses. The app allows users to log details such as expense description, amount, date, and category. It also uses **Machine Learning** with **ML.NET** to predict next month's expenses based on historical data.

![Screenshot](https://github.com/rasoolzada/SmartExpenseTracker/blob/master/SmartExpenseTracker/wwwroot/expense.gif)

## Features
- Track and log expenses with details (description, amount, date, category)
- **Machine Learning** predictions for next month's expenses using **ML.NET**
- Data storage with **SQL Server** and **Entity Framework**
- Simple and user-friendly interface for managing your finances

## Tech Stack
- **ASP.NET Core** for the web application
- **Entity Framework** for database management
- **ML.NET** for machine learning predictions
- **SQL Server** for data storage

## Data Requirements for Machine Learning
- To get accurate predictions, a **minimum of 100 expense entries** is recommended.
- More data (e.g., 200-300 rows or more) will improve the quality of the machine learning model's predictions, as it provides more patterns and variability for training.
  
**Note**: Ensure to log a variety of expenses across different categories to help the machine learning model learn better patterns.

## Installation

### Prerequisites
- .NET SDK (preferably .NET 6 or higher)
- SQL Server or SQL Server Express

### Steps to Set Up Locally
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/SmartExpenseTracker.git
   cd SmartExpenseTracker
### Update the `appsettings.json` file with your database connection string:

Example connection string for SQL Server:
```json
"ConnectionStrings": {
  "ExpenseTrackerDatabase": "Server=.\\SQLEXPRESS;Database=ExpenseTrackerDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```
### Apply Migration

### Usage
- Access the app at [http://localhost:7021](http://localhost:7021) in your browser.
- Log your expenses (ensure you have at least 100 entries for better predictions).
- The app will use historical data to predict next month’s expenses.

### Contributing
Contributions are welcome! If you have suggestions for improvements or new features, feel free to fork the repository and submit a pull request. You can also open issues to discuss bugs or request features.

