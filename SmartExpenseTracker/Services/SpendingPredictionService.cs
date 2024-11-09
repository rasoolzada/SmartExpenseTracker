using Microsoft.ML;
using Microsoft.ML.Data;
using SmartExpenseTracker.Data;


namespace SmartExpenseTracker.Services
{
    public class SpendingPredictionService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public SpendingPredictionService(ExpenseTrackerDbContext context)
        {
            _context = context;
            _mlContext = new MLContext();
            TrainModel();
        }

        private void TrainModel()
        {
            // Fetch expense data for the last 12 months for model training
            var expenses = _context.Expenses
                .Where(e => e.Date >= DateTime.Now.AddMonths(-12))
                .Select(e => new ExpenseData
                {
                    Amount = (float)e.Amount,
                    Month = e.Date.Month,
                    Year = e.Date.Year
                })
                .ToList();

            if (!expenses.Any())
                return;

            // Log the training data to ensure it's being loaded properly
            foreach (var expense in expenses)
            {
                Console.WriteLine($"Amount: {expense.Amount}, Month: {expense.Month}, Year: {expense.Year}");
            }

            // Load the data into an IDataView
            var dataView = _mlContext.Data.LoadFromEnumerable(expenses);

            // Define the data processing and training pipeline
            var pipeline = _mlContext.Transforms
                .Conversion.ConvertType("Month", outputKind: DataKind.Single)
                .Append(_mlContext.Transforms.Conversion.ConvertType("Year", outputKind: DataKind.Single))
                .Append(_mlContext.Transforms.Concatenate("Features", "Month", "Year"))
                .Append(_mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Amount"))
                .Append(_mlContext.Regression.Trainers.FastTree());

            // Train the model
            _model = pipeline.Fit(dataView);

            // Evaluate the model to check performance (optional)
            var predictions = _model.Transform(dataView);
            var metrics = _mlContext.Regression.Evaluate(predictions);
            Console.WriteLine($"R-Squared: {metrics.RSquared}");
        }

        // Predict the next month's total spending
        public decimal PredictNextMonthSpending()
        {
            if (_model == null)
                return 0;

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ExpenseData, ExpensePrediction>(_model);

            // Predict for next month
            var nextMonth = DateTime.Now.AddMonths(1);
            var predictedTotalAmount = 0f;

            // Fetch all expenses for the last 12 months (ignoring categories)
            var expenses = _context.Expenses
                .Where(e => e.Date >= DateTime.Now.AddMonths(-12)) // Use the same data as in training
                .ToList();

            foreach (var expense in expenses)
            {
                var prediction = predictionEngine.Predict(new ExpenseData
                {
                    Month = nextMonth.Month,
                    Year = nextMonth.Year
                });

                predictedTotalAmount += prediction.PredictedAmount;
            }

            // Return the total predicted amount for the next month
            return (decimal)predictedTotalAmount;
        }

        // Predict the average spending for all items in the dataset
        public decimal PredictAverageSpendingForAllItems()
        {
            if (_model == null)
                return 0;

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ExpenseData, ExpensePrediction>(_model);

            // Fetch all expenses from the database for prediction
            var expenses = _context.Expenses
                .Where(e => e.Date >= DateTime.Now.AddMonths(-12)) // Only expenses from the last 12 months
                .Select(e => new ExpenseData
                {
                    Amount = (float)e.Amount,
                    Month = e.Date.Month,
                    Year = e.Date.Year
                })
                .ToList();

            if (!expenses.Any())
                return 0;

            // Sum of predicted expenses for all items
            float totalPredictedAmount = 0;
            int count = 0;

            foreach (var expense in expenses)
            {
                var prediction = predictionEngine.Predict(expense);
                totalPredictedAmount += prediction.PredictedAmount;
                count++;
            }

            // Calculate and return the average of the predicted expenses
            return count > 0 ? (decimal)(totalPredictedAmount / count) : 0;
        }
    }

    // Define the data structure for training and prediction
    public class ExpenseData
    {
        public float Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class ExpensePrediction
    {
        [ColumnName("Score")]
        public float PredictedAmount { get; set; }
    }
}
