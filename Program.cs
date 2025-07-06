using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Numerics;
using System.Transactions;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Reflection.PortableExecutable;
using System.Collections;

namespace MiniBankProject
{
    internal class Program

    {
        // Constants
        const double MinimumBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string ReviewsFilePath = "reviews.txt";
        const string RequestsFilePath = "requests.txt";
        const string CancelcreateAccountRequestsFilePath = "CancleRequests.txt";
        const string AdminInformationFilePath = "Admin.txt";
        const string MonthlyStatementGeneratorFilePath = "Statement_Acc12345_2025-07.txt";
        // Export All Account Info file path
        static string ExportFilePath = "ExportedAccounts.txt";
        // File to store User Feedbacks
        static string UserFeedbackFilePath = "UserFeedbacks.txt";
        // Backup file name with timestamp
        string backupFileName = $"Backup_{DateTime.Now:yyyy-MM-dd_HHmm}.txt";

        // File accept loan requests
        static string LoanRequestsFilePath = "LoanRequests.txt";
        // file to store cancel loan requests

        // User vrequest Appointment File
        static string AppointmentRequestsFilePath = "AppointmentRequests.txt";

        // generate ID number for every account 
        static int LastAccountNumber = 0;
        static int IndexID = 0;
        // Global lists(parallel)
        static List<int> AccountNumbers = new List<int>();

        // Account data in Lists
        static List<string> AccountUserNames = new List<string>();
        static List<string> AccountUserNationalID = new List<string>();
        static List<double> UserBalances = new List<double>();
        static List<string> AccountUserHashedPasswords = new List<string>();
        static List<string> UserPhoneNumbers = new List<string>();
        static List<string> UserAddresses = new List<string>();
        // String list to store user feedbacks
        static List<string> UserFeedbacks = new List<string>();
        // List to store user IDs for quick access
        static List<bool> UserIsLocked = new List<bool>();


        // Store appointment requests as "IndexID|ServiceType|DateTime"
        static Queue<string> AppointmentRequests = new Queue<string>();
     

        // Track if a user has an active appointment
        static List<bool> UserHasActiveAppointment = new List<bool>();
        // Optionally store the appointment datetime for each user
        static List<DateTime> UserAppointmentDates = new List<DateTime>();

        // User Transactions History
        static List<List<string>> UserTransactions = new List<List<string>>(); // MonthlyStatementGenerator



        // generate ID number for Admin account 
        static int LastAdminAccountNumber = 0;

        // Admin Login information 
        static List<int> AdminAccountNumber = new List<int>();
        static List<string> AdminName = new List<string>();
        static List<string> AdminID = new List<string>();
        static List<string> AccountAdminHashedPasswords = new List<string>();



        //Requests in queue
        static Queue<string> createAccountRequests = new Queue<string>(); // format: "Name|NationalID"
        static Queue<string> CancelcreateAccountRequests = new Queue<string>(); // format: "Name|NationalID"

        //review in stack
        static Stack<string> UserReviewsStack = new Stack<string>();

        

        // Loan requests 
        static List<bool> UserHasActiveLoan = new List<bool>();
        static List<double> UserLoanAmounts = new List<double>();
        static List<double> UserLoanInterestRates = new List<double>();
        static Queue<string> LoanRequests = new Queue<string>();

        // Fixed Currency Exchange Rates
        static readonly Dictionary<string, double> ExchangeRates = new Dictionary<string, double>
        {
            {"USD", 3.8},
            {"EUR", 4.1},
            {"OMR", 1.0},
            {"UAE", 10.0}
        };



        // ======================================== Menu Functions =================================
        static void Main(string[] args)
        {
            Console.WriteLine("========== Wellcome To Bank System =======================");
            Console.ReadLine();
            LoadAccountsInformationFromFile();
            LoadReviews();
            LoadAcceptedRequests();
            LoadAdminInformationFromFile();
            LoadCancleRequests();
            LoadAppointmentRequestsFromFile();

            bool UsersSystemMenu = true;
            // while loop to display the mnue ewhile the flag is true
            while (UsersSystemMenu)
            {
                // display menu user of system list
                Console.Clear();
                Console.WriteLine("Select User Types:");
                Console.WriteLine("1. User");
                Console.WriteLine("2. Admin");
                Console.WriteLine("0. Exist");
                char choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                // user switch method to select one of many code blocks to be executed.
                switch (choice)
                {
                    // case to display user menu
                    case '1':
                        UserMenu();
                        break;
                    // case to display Admin menu
                    case '2':
                        AdminMenu();
                        break;
                    // case to Exist from whole system
                    case '0':
                        SaveAccountsInformationToFile();
                        SaveReviews();
                        SaveAccepteRequestsToFaile();
                        SaveAdminInformationToFile();
                        SaveCancleRequestsToFaile();

                        // save all data in backup file before exit from system
                        BackupAllDataToFile();

                        UsersSystemMenu = false;
                        break;
                    // by default case to display error choic message 
                    default:
                        Console.WriteLine("The choice number is uncorrect, please try agine");
                        Console.ReadKey();
                        break;
                }
            }
        }

        


        // User Menu
        public static void UserMenu()
        {
            bool inUserMenu = true;
            // while loop to display the mnue ewhile the flag is true 
            while (inUserMenu)
            {
                Console.Clear();
                Console.WriteLine("\n------ User Menu ------");
                Console.WriteLine("1. Request Account Creation");
                Console.WriteLine("2. Login");
                Console.WriteLine("0. exist");
                Console.Write("Select option: ");
                char userChoice = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (userChoice)
                {
                    // case to Request Account Creation
                    case '1':
                        RequestAccountCreation();
                        Console.ReadLine();
                        break;
                    // case to Deposit
                    case '2':
                        IndexID = UserLoginWith_ID_Password();

                        if (IndexID != -1)
                        {
                            Console.WriteLine($"{AccountUserNames[IndexID]} Login successfully");
                            Console.ReadLine();
                            UserMenuOperations(IndexID);
                            Console.ReadLine();

                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        break;

                    // case to exist from user menu and Return to Main Menu 
                    case '0':
                        inUserMenu = false; // this will exit the loop and return
                        break;
                    // default case if user choice the wronge number within the range of cases 
                    default:
                        Console.WriteLine("Wronge Choice number, Try Agine!");
                        Console.ReadKey();
                        break;
                }
            }


        }

        // Admin Menu
        public static void AdminMenu()
        {
            bool InAdminMenu = true;
            // while loop to display the mnue ewhile the flag is true 
            while (InAdminMenu)
            {
                Console.Clear();
                Console.WriteLine("\n------ Admin Menu ------");
                Console.WriteLine("1. create account");
                Console.WriteLine("2. Login");
                Console.WriteLine("0. exist");
                Console.Write("Select option: ");
                char userChoice = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (userChoice)
                {
                    // case to Request Account Creation
                    case '1':
                        AdminCreateAccount();
                        Console.ReadLine();
                        break;
                    // case to Deposit
                    case '2':
                        IndexID = AdminLoginWith_ID_Password();
                        if (IndexID !=-1)
                        {
                            Console.WriteLine($"{AdminName[IndexID]} Successfully Login");
                            Console.ReadLine();
                            AdminMenuOperations();
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID or password.");
                        }
                        break;

                    // case to exist from user menu and Return to Main Menu 
                    case '0':
                        InAdminMenu = false; // this will exit the loop and return
                        break;
                    // default case if user choice the wronge number within the range of cases 
                    default:
                        Console.WriteLine("Wronge Choice number, Try Agine!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ===================== User Features Function ==========================
        // Request Account Creation function
        public static void RequestAccountCreation()
        {
            string UserName = "";
            string name = "";
            bool ValidName = true;
            string UserID = "";
            bool ValidID = true;
            string password = "";
            string hashedPassword = "";
            string UserPhoneNumber = "";
            bool IsValidPhone = true;
            string UserAddress = "";
            // Initialize a boolean flag to control the save process.
            bool IsValidAddress = true;
            bool IsSave = true;
            int tries = 0;
            
            // Error handling 
            try
            {
                // 1. Enter User Name Process
                do
                {
                    // ask user to enter his name
                    Console.WriteLine("Enter Your Name: ");
                    name = Console.ReadLine();
                    // valid the name input 
                    ValidName = stringOnlyLetterValidation(name);
                    if(ValidName== true)
                    {
                        UserName = name;
                        IsSave = true;
                    }
                    else
                    {
                        IsSave = false;
                        tries++;
                    }
                    
                } while (ValidName == false && tries <3);
                // check if user tries to enter valid name more than 3 times
                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    Console.ReadLine();
                    return;
                }
                tries = 0;
                // 2. Enter User ID Process
                do
                {
                    // ask user to enter his national ID 
                    Console.WriteLine("Enter your National ID: ");
                    string ID = Console.ReadLine();

                    // check if number already exist in the list of accounts
                    bool IDExist = CheckUserIDExist(ID);
                    if (IDExist)
                    {
                        Console.WriteLine("This National ID already exists. Please enter a different ID.");
                        IsSave = false;
                        tries++;
                    }
                    else
                    {


                        // valid the ID input
                        ValidID = IDValidation(ID);
                        // check if 

                        if (ValidID == true)
                        {
                            UserID = ID;
                            IsSave = true;
                        }
                        else
                        {
                            IsSave = false;
                            tries++;
                        }
                        
                    }
                } while (IsSave == false && tries < 3);
                // check if user tries to enter valid ID more than 3 times
                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    Console.ReadLine();
                    return;
                }
                // reset tries to 0 for next validation
                tries = 0;
                // 3. Set Password with masking + hashing
                do
                {
                    Console.Write("Set your password: ");
                    password = ReadPassword(); // masked input
                    hashedPassword = HashPassword(password);
                    if (string.IsNullOrEmpty(hashedPassword))
                    {
                        Console.WriteLine("Password cannot be empty. Please try again.");
                        IsSave = false; 
                        tries++;
                    }
                    else
                    {
                        IsSave = true;
                    }
                } while (IsSave == false && tries < 3); // Ensure password is not empty

                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    Console.ReadLine();
                    return;
                }
                tries = 0;
                //4. Set Phone Number 
                do
                {
                    Console.WriteLine("Enter your phone number: ");
                    UserPhoneNumber = Console.ReadLine();
                    IsValidPhone = IsValidPhoneNumber(UserPhoneNumber);
                    if (IsValidPhone == false)
                    {
                        Console.WriteLine("Invalid phone number format. Please enter a valid phone number.");
                        IsSave = false;
                        tries++;
                    }
                    else
                    {
                        IsSave = true;
                    }



                } while (IsSave == false && tries < 3);
                if (tries == 3) {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    Console.ReadLine();
                    return;
                }
                tries = 0;
                // 5. Set Address
                do
                {
                    Console.WriteLine("Enter your address: ");
                    UserAddress = Console.ReadLine();
                    // check if address is valid
                    IsValidAddress = StringlettersWithNumbers(UserAddress);
                    if (IsValidAddress == false)
                    {
                        Console.WriteLine("Invalid address format. Please enter a valid address.");
                        IsSave = false;
                        tries++;
                    }
                    else
                    {
                        IsSave = true;
                    }
                } while(IsSave == false && tries < 3);
                if (tries == 3) {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    Console.ReadLine();
                    return;
                }
                // reset tries to 0 for next validation
                tries = 0;
                // 6. Save information in the User lists
                if (IsSave == true )
                {
                    bool AlreadyRequested = false;
                    // loop in queue to chech if request with same id already submit to Prevent Duplicate Account Requests 
                    foreach (string Request in createAccountRequests)
                    {
                        // split request to get the user id number
                        string[] splitRequest = Request.Split("|");
                        //check if id in rqures queue id exist or not 
                        if (splitRequest[1] == UserID)
                        {
                            // if yes put AlreadyRequested flag with true value
                            AlreadyRequested = true;
                            break;
                        }

                    }
                    // based on AlreadyRequested flad we decided if we save user inputes of account information or not 
                    if (AlreadyRequested)
                    {
                        Console.WriteLine("Your request is already waiting for confirmation.");
                    }
                    else
                    {
                        // save request in queue
                        string request = UserName + "|" + UserID + "|" + hashedPassword + "|" + UserPhoneNumber + "|" + UserAddress;
                        createAccountRequests.Enqueue(request);
                        Console.WriteLine("Request Account Creation successfully submitted.");
                        UserFeedbackSystem("Request Creation Account");
                    }
                }
                
                
            }
            catch
            {
                // display message submit failed 
                Console.WriteLine("Request Account Creation failed submit");
            }

        }
        // User Menu Operation
        public static void UserMenuOperations(int IndexID)
        {
            bool inUserMenu = true;
            // while loop to display the mnue ewhile the flag is true 
            while (inUserMenu)
            {
                Console.Clear();
                Console.WriteLine("\n------ User Menu Operation ------");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. View Balance");
                Console.WriteLine("4. Submit Review/Complaint");
                Console.WriteLine("5. Transfer Money");
                Console.WriteLine("6. Undo Last Complaint");
                Console.WriteLine("7. Update Phone Number and Address");
                Console.WriteLine("8. View Transaction");
                Console.WriteLine("9. Request a Loan");
                Console.WriteLine("10. View Active Loan Information");
                Console.WriteLine("11. Book Appointment For Book Service");
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select option: ");
                string userChoice = Console.ReadLine();

                switch (userChoice)
                {                    
                    // case to Deposit
                    case "1":
                        
                        Console.WriteLine("Proceeding to deposit...");
                        Deposit(IndexID); // If user exists, proceed with deposit
                        Console.ReadLine(); // Wait for user input before continuing
                      
                        break;
                    // case to Withdraw
                    case "2":
                        
                        Console.WriteLine("Proceeding to withdraw...");
                        withdraw(IndexID); // If user exists, proceed with withdraw
                        Console.ReadLine(); // Wait for user input before continuing
                       
                        break;
                    // case to View Balance
                    case "3":

                        Console.WriteLine("Proceeding to Check Balance...");
                        CheckBalance(IndexID); // If user exists, proceed with chech balance
                        Console.ReadLine(); // Wait for user input before continuing
                       
                        break;
                    // case to Submit Review/Complaint
                    case "4":
                        SubmitReview();
                        Console.ReadLine();
                        break;
                    // Transfer Money
                    case "5":
                        // Ask user to enter the National ID of the account to transfer money to
                        Console.WriteLine("Enter the National ID of the account you want to transfer money to.....");
                        int UserIndexID2 = EnterUserID();
                        if (UserIndexID2 != -1 && UserIndexID2 != IndexID) // when user login to its account by accountID number, this number save in value IndexID which decalre in "internal calss program" so when want to transer from its account to another account, no need to enter its accountIDNumber agine it save temberary in variable "IndexID"
                        {
                            Transfer(IndexID, UserIndexID2); // If user exists, proceed with transfer
                        }
                        else if (UserIndexID2 == IndexID)
                        {
                            Console.WriteLine("You cannot transfer money to your own account.");
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check the National ID of the recipient.");
                        }
                        Console.ReadLine(); // Wait for user input before continuing
                        break;
                    // Undo Last Complaint
                    case "6":
                        UndoLastComplaint();
                        Console.ReadLine(); // Wait for user input before continuing
                        break;
                    // case to Update Phone Number and Address
                    case "7":
                        UpdatePhoneAndAddress(IndexID); // If user exists, proceed with update
                        Console.ReadLine(); // Wait for user input before continuing
                        break;
                    // case to View Transaction History
                    case "8":
                        Console.WriteLine("Which option do you want to view transaction: ");
                        Console.WriteLine("1. Display All Your Transaction");
                        Console.WriteLine("2. Display Transaction For Specific Manth on a Specific Year");
                        Console.WriteLine("3. View Last N Transactions");
                        Console.WriteLine("4. View Transactions After a Date");
                        Console.WriteLine("0. Return to User Menu");
                        Console.Write("Select option: ");
                        string choice = Console.ReadLine();
                        Console.WriteLine();

                        // Use switch to select one of many code blocks to be executed
                        switch(choice)
                        {
                            // case to Display All Your Transaction
                            case "1":
                                 PrintAllTransactions(IndexID);
                                break;
                            // case to Display Transaction For Specific Manth on a Specific Year
                            case "2":
                                GenerateMonthlyStatement(IndexID);
                                break;
                            // case to View Last N Transactions
                            case "3":
                                ViewLastNTransactions(IndexID);
                                Console.ReadLine();
                                break;
                            // case to View Transactions After a Date
                            case "4":
                                ViewTransactionsAfterDate(IndexID);
                                Console.ReadLine();
                                break;
                            case "0":
                                // Return to User Menu
                                inUserMenu = true; // this will exit the loop and return
                                break;
                            // default case if user choice the wronge number within the range of cases 
                            default:
                                Console.WriteLine("Wronge Choice number, Try Agine!");
                                break;
                        }
                        Console.ReadLine();
                        break;
                    // case to Request a Loan
                    case "9":
                        RequestLoan(IndexID);
                        Console.ReadLine();
                        break;
                    // case to View Active Loan Information
                    case "10":
                        ViewActiveLoanInfo(IndexID);
                        Console.ReadLine();
                        break;
                    // case to Book Appointment For Book Service
                    case "11":           
                        RequestBookAppointment(IndexID);
                        Console.ReadLine();
                        break;

                    // case to exist from user menu and Return to Main Menu 
                    case "0":
                        inUserMenu = false; // this will exit the loop and return
                        break;
                    // default case if user choice the wronge number within the range of cases 
                    default:
                        Console.WriteLine("Wronge Choice number, Try Agine!");
                        Console.ReadKey();
                        break;
                }
            }
        }
        // Update Phone number and address Function
        public static void UpdatePhoneAndAddress(int IndexID)
        {
            // Ask user to select which data want to update 
            Console.WriteLine("Select the data you want to update:");
            Console.WriteLine("1. Phone Number");
            Console.WriteLine("2. Address");
            Console.WriteLine("0. Return to User Menu");
            Console.Write("Select option: ");
            char choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            // Use switch to select one of many code blocks to be executed
            Console.WriteLine("You selected: " + choice);
            Console.WriteLine("========================================");
            switch (choice)
            {
                // case to Update Phone Number
                case '1':
                    Console.WriteLine("Enter your new phone number: ");
                    string newPhoneNumber = Console.ReadLine();
                    // Validate the new phone number
                    if (IsValidPhoneNumber(newPhoneNumber))
                    {
                        UserPhoneNumbers[IndexID] = newPhoneNumber; // Update the phone number in the list
                        Console.WriteLine($"Updated Phone: {UserPhoneNumbers[IndexID]}");
                        UserFeedbackSystem("Updated Phone");
                    }
                    else
                    {
                        Console.WriteLine("Invalid phone number format. Please try again.");
                    }
                    break;
                // case to Update Address
                case '2':
                    Console.WriteLine("Enter your new address: ");
                    string newAddress = Console.ReadLine();
                    // Validate the new address
                    if (StringlettersWithNumbers(newAddress))
                    {
                        UserAddresses[IndexID] = newAddress; // Update the address in the list
                        Console.WriteLine($"Updated Address: {UserAddresses[IndexID]}");
                        UserFeedbackSystem("Updated Address");

                    }
                    else
                    {
                        Console.WriteLine("Invalid address format. Please try again.");
                    }
                    break;
                // case to Return to User Menu
                case '0':
                    return; // Exit the method and return to User Menu
                // default case if user choice the wronge number within the range of cases 
                default:
                    Console.WriteLine("Wronge Choice number, Try Agine!");
                    break;
            }
        }
        // Deposit Function 
        public static void Deposit(int IndexID)
        {
            if (IndexID < 0 || IndexID >= UserBalances.Count)
            {
                Console.WriteLine("Error: Invalid user index for deposit operation.");
                return;
            }

            int tries = 0;
            // Initialize a boolean flag to control the deposit loop.
            bool IsDeposit = false;
            // Initialize a variable to store the final parsed deposit amount.
            double FinalDepositAmount = 0.0;
            // Initialize an index to find the user's position in the account list.

            // Start a try block to catch potential runtime exceptions.
            try
            {
                // Repeat until a valid deposit is made.
                do
                {
                    Console.WriteLine("Select deposit currency: \n1. OMR\n2. USD\n3. EUR\n4. UAE");
                    string choice = Console.ReadLine();
                    string currency = choice == "1" ? "OMR" : choice == "2" ? "USD" : choice == "3" ? "EUR" : choice == "4" ? "UAE": "";

                    if (string.IsNullOrEmpty(currency))
                    {
                        Console.WriteLine("Invalid selection.");
                        return;
                    }
                 
                    Console.WriteLine($"Enter the amount to deposit in {currency}: ");
                    if (!double.TryParse(Console.ReadLine(), out double originalAmount) || originalAmount <= 0)
                    {
                        Console.WriteLine("Invalid deposit amount.");
                        return;
                    }
                    // convert double to string 
                    string DepositAmount = originalAmount.ToString("F2"); // Format to 2 decimal places

                    // Validate the entered amount using a custom method.
                    bool ValidDepositAmount = AmountValid(DepositAmount);
                    if (ValidDepositAmount == false)
                    {
                        // Display error if the input is not valid.
                        Console.WriteLine("Invalid deposit Amount Format, should be  00.00");
                        IsDeposit = false;
                        tries++;
                    }
                    // If input is valid, find the user index.
                    else
                    {
                        // convert string to double using TryParse
                        double.TryParse(DepositAmount, out FinalDepositAmount);

                        // Convert to OMR
                        double rate = ExchangeRates[currency];
                        double convertedAmount = FinalDepositAmount / rate;

                        Console.WriteLine($"Converted Amount from {currency} : {FinalDepositAmount} to OMR: {convertedAmount:F2}");

                        // Update the user's balance by adding the deposit amount.
                        UserBalances[IndexID] = UserBalances[IndexID] + convertedAmount;

                        // Display success message and the new balance.
                        Console.WriteLine($"Successfully deposited {convertedAmount} {currency} to your account.");
                        PrintReceipt(transactionType: "Deposit", amount: convertedAmount, balance: UserBalances[IndexID]);
                        // Set the flag to true to exit the loop.
                        IsDeposit = true;
                        
                        // Record the transaction in the user's transaction history.
                        string transactionRecord = $"{AccountUserNationalID[IndexID]},{DateTime.Now:yyyy-MM-dd},Deposit, {FinalDepositAmount},{convertedAmount},{UserBalances[IndexID]}";
                        for (int i = UserTransactions.Count; i < UserBalances.Count; i++)
                        {
                            UserTransactions.Add(new List<string>());
                        }

                        UserTransactions[IndexID].Add(transactionRecord);
                        // Save the user's transactions to a file.
                        SaveUserTransactionsToFile();
                        UserFeedbackSystem("Deposit");
                        // Exit the method (if inside a method).
                        return;

                    }
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (IsDeposit == false && tries < 3);
        }
            //Print any exception message that occurs during execution.
            catch (Exception e) { Console.WriteLine(e.Message); }

}
        // Withdraw Function 
        public static void withdraw(int IndexID)
        {
            int tries = 0;
            // Initialize a boolean flag to control the deposit loop.
            bool IsWithdraw = false;
            // Initialize a variable to store the final parsed deposit amount.
            double FinalwithdrawAmount = 0.0;
            // declare variable print balance after any process
            double BalanceAfterProcess = 0.0;
            // Start a try block to catch potential runtime exceptions.
            try
            {
                // Repeat until a valid deposit is made.
                do
                {
                    
                    Console.WriteLine("Enter the amount of money you want to withdrw from your balance: ");
                    string WithdrawAmount = Console.ReadLine();
                    // Validate the entered amount using a custom method.
                    bool ValidWithAmount = AmountValid(WithdrawAmount);
                    if (ValidWithAmount == false)
                    {
                        // Display error if the input is not valid.
                        Console.WriteLine("Invalid input");
                        IsWithdraw = false;
                        tries++;
                    }
                    // If input is valid, find the user index.
                    else
                    {

                        // convert string to double using TryParse
                        double.TryParse(WithdrawAmount, out FinalwithdrawAmount);
                        // check if user balamce is less than or equal MinimumBalance
                        bool checkBalance = CheckBalanceAmount(FinalwithdrawAmount, IndexID);
                        if (checkBalance == true)
                        {
                            // Update the user's balance by adding the deposit amount.
                            UserBalances[IndexID] = UserBalances[IndexID] - FinalwithdrawAmount;
                            Console.WriteLine($"Successfully withdraw");
                            Console.WriteLine($"Your Current Balance is {UserBalances[IndexID]}");
                            // Record the transaction in the user's transaction history.
                            PrintReceipt(transactionType: "Withdraw", amount: FinalwithdrawAmount, balance: UserBalances[IndexID]);         
                            // Record the transaction in the user's transaction history.
                            string transactionRecord = $"{DateTime.Now:yyyy-MM-dd},Withdraw,{FinalwithdrawAmount},{UserBalances[IndexID]}";
                            for (int i = UserTransactions.Count; i < UserBalances.Count; i++)
                            {
                                UserTransactions.Add(new List<string>());
                            }

                            UserTransactions[IndexID].Add(transactionRecord);
                            // Save the user's transactions to a file.
                            SaveUserTransactionsToFile();
                            UserFeedbackSystem("Withdraw");
                            // Set the flag to true to exit the loop.
                            IsWithdraw = true;
                        }
                        else
                        {
                            BalanceAfterProcess = UserBalances[IndexID] - FinalwithdrawAmount;
                            Console.WriteLine($"Can not withdraw {FinalwithdrawAmount} from your balance, becouse your balance after with draw is {BalanceAfterProcess} which less than 100.00");
                        }
                        return;

                    }
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (IsWithdraw == false && tries<3);
            }
            //Print any exception message that occurs during execution.
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
        // Check Balance Function
        public static void CheckBalance(int IndexID)
        {
            Console.WriteLine($"Your Current Balance is {UserBalances[IndexID]}");
        }
        // Submit Review Function 
        public static void SubmitReview()
        {
            //error handling using try-catch
            try
            {
                // ask user to enter their review
                Console.WriteLine("Enter Your Review");
                string review = Console.ReadLine();
                //check valid the input review 
                bool ValidReview = StringlettersWithNumbers(review);
                if(ValidReview == true)
                {// push review in stack named "UserReviews"
                    UserReviewsStack.Push(review);
                    Console.WriteLine("Your Review successfully submited");
                    UserFeedbackSystem("Submit Review");
                }
                else
                {
                    Console.WriteLine("Unvalid input review");
                }
                
            }
            catch
            {
                //display massage if review 
                Console.WriteLine("Failed Submitted, try agine!");
            }
        }
        // login user ID and password 
        public static int UserLoginWith_ID_Password()
        {
            int UserIndex = -1;
            int indexID = EnterUserID();
            if (indexID != -1)
            {
                UserIndex = EnterUserPassword(indexID);
            }
            else
            {
                Console.WriteLine("Login failed. Please check your National ID.");
                UserIndex = - 1; // Return -1 to indicate login failure
            }
            return UserIndex;
        }
        /*
         Transfer money
            • Let users transfer money between two account numbers. 
            • Check balance constraints on sender before transferring.
         */
        public static void Transfer(int UserIndexID, int UserIndexID2)
        {
            string TransferAmount = "";
            double FinalTransferAmount = 0.0;
            bool IsTransfer = false;
            int tries = 0;

            try
            {
                Console.WriteLine("Select Recive currency: \n1. OMR\n2. USD\n3. EUR\n4. UAE");
                string choice = Console.ReadLine();
                string currency = choice == "1" ? "OMR" : choice == "2" ? "USD" : choice == "3" ? "EUR" : choice == "4" ? "UAE" : "";
                do
                {

                    Console.WriteLine("Enter the amount of money you want to transfer: ");
                    TransferAmount = Console.ReadLine();

                    if (!AmountValid(TransferAmount))
                    {
                        Console.WriteLine("Invalid input.");
                        tries++;
                    }
                    else
                    {
                        double.TryParse(TransferAmount, out FinalTransferAmount);

                        if (CheckBalanceAmount(FinalTransferAmount, UserIndexID))
                        {
                            UserBalances[UserIndexID] -= FinalTransferAmount;

                            // Convert to Recive currency 
                            double rate = ExchangeRates[currency];
                            double convertedAmount = FinalTransferAmount * rate;
                            Console.WriteLine($"Converted Amount from OMR: {FinalTransferAmount} to {currency}: {convertedAmount:F2}");

                            // Update the receiver's balance
                            UserBalances[UserIndexID2] += convertedAmount;

                            Console.WriteLine($"Successfully transferred {FinalTransferAmount} from Account {AccountNumbers[UserIndexID]} to Account {AccountNumbers[UserIndexID2]}");
                            Console.WriteLine($"Your Current Balance is {UserBalances[UserIndexID]}");
                            Console.ReadLine();
                            string transactionRecord = $"{DateTime.Now:yyyy-MM-dd},Transfer From,{FinalTransferAmount},{UserBalances[UserIndexID]},To:{AccountNumbers[UserIndexID2]}, Converted to {convertedAmount}{currency}";
                            string transactionRecord2 = $"{DateTime.Now:yyyy-MM-dd},Transfer To,{FinalTransferAmount},{UserBalances[UserIndexID2]},From:{AccountNumbers[UserIndexID]}";

                            for (int i = UserTransactions.Count; i < UserBalances.Count; i++)
                                UserTransactions.Add(new List<string>());

                            UserTransactions[UserIndexID].Add(transactionRecord);     // Correct sender record
                            UserTransactions[UserIndexID2].Add(transactionRecord2);   // Correct receiver record

                            SaveUserTransactionsToFile();
                            UserFeedbackSystem("Transfer");

                            IsTransfer = true;
                        }
                        else
                        {
                            Console.WriteLine($"Cannot transfer {FinalTransferAmount}. Insufficient balance (must remain above 100.00).");
                        }

                        return; // Exit after handling transfer
                    }

                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the allowed number of attempts.");
                        return;
                    }

                } while (!IsTransfer && tries < 3);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        // Undo Last Complaint Submitted 
        public static void UndoLastComplaint()
        {
            // Check if there are any reviews in the stack
            if (UserReviewsStack.Count > 0)
            {
                // Pop the last review from the stack
                string lastReview = UserReviewsStack.Pop();
                Console.WriteLine($"Last review '{lastReview}' has been undone.");
            }
            else
            {
                Console.WriteLine("No reviews to undo.");
            }
        }

        // Print Receipt After Deposit/Withdraw 
        public static void PrintReceipt(string transactionType, double amount, double balance)
        {
            Console.WriteLine("\n--- Transaction Receipt ---");
            Console.WriteLine($"Transaction Type: {transactionType}");
            Console.WriteLine($"Amount: {amount}");
            Console.WriteLine($"New Balance: {balance}");
            Console.WriteLine("---------------------------\n");
        }

        // Generate Monthly Statement
        public static void GenerateMonthlyStatement(int IndexID)
        {
            LoadUserTransactionsFromFile();

            Console.WriteLine("Enter year (e.g., 2025): ");
            string yearInput = Console.ReadLine();
            Console.WriteLine("Enter month (1-12): ");
            string monthInput = Console.ReadLine();

            if (!int.TryParse(yearInput, out int year) || !int.TryParse(monthInput, out int month) ||
                month < 1 || month > 12)
            {
                Console.WriteLine("Invalid year or month input.");
                return;
            }

            var transactions = UserTransactions[IndexID]
                .Where(t =>
                {
                    var parts = t.Split(',');
                    if (DateTime.TryParse(parts[0], out DateTime date))
                    {
                        return date.Year == year && date.Month == month;
                    }
                    return false;
                }).ToList();

            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions found for the selected month/year.");
                return;
            }

            
            Console.WriteLine($"\n--- Transaction History for Account: {AccountNumbers[IndexID]} ---");
            Console.WriteLine($"Period: {year}-{month:D2}");
            Console.WriteLine("Date, Type, Amount, Balance After Transaction");
            Console.WriteLine("---------------------------------------------");

            foreach (var line in transactions)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("\nEnd of transactions for the selected month.");
            UserFeedbackSystem("Generate Monthly Statement");
        }

        // Request a Loan Function
        public static void RequestLoan(int IndexID)
        {
            if (UserHasActiveLoan[IndexID])
            {
                Console.WriteLine("You already have an active loan. Cannot request another loan.");
                return;
            }
            if (UserBalances[IndexID] < 5000)
            {
                Console.WriteLine("Your balance must be at least 5000 to request a loan.");
                return;
            }

            Console.Write("Enter desired loan amount: ");
            if (!double.TryParse(Console.ReadLine(), out double loanAmount) || loanAmount <= 0)
            {
                Console.WriteLine("Invalid loan amount.");
                return;
            }

            // Format: UserIndex|LoanAmount
            string request = $"{IndexID}|{loanAmount}";
            LoanRequests.Enqueue(request);

            Console.WriteLine("Loan request submitted for admin approval.");
            UserFeedbackSystem("Request Loan");
        }

        // View Active Loan Information Function

        public static void ViewActiveLoanInfo(int IndexID)
        {
            if (UserHasActiveLoan[IndexID])
            {
                double remainingLoan = UserLoanAmounts[IndexID];
                double monthlyPayment = 150.0;
                int monthsToPayOff = (int)Math.Ceiling(remainingLoan / monthlyPayment);

                Console.WriteLine($"You have an active loan of {remainingLoan} OMR with an interest rate of {UserLoanInterestRates[IndexID]}%.");
                Console.WriteLine($" you pay 150 OMR every month, thus,  you pay off your loan in {monthsToPayOff} month(s).");
            }
            else
            {
                Console.WriteLine("You do not have any active loans.");
            }
        }

        // Auto Repay Loan If Applicable Function
        public static void AutoRepayLoanIfApplicable(int IndexID)
        {
            // Auto-repay only on the 1st day of each month
            if (DateTime.Now.Day != 1)
                return;

            // Check if the user has an active loan
            if (!UserHasActiveLoan[IndexID])
                return;

            double repaymentAmount = 150.0;

            if (UserBalances[IndexID] >= repaymentAmount)
            {
                UserBalances[IndexID] -= repaymentAmount;
                UserLoanAmounts[IndexID] -= repaymentAmount;

                if (UserLoanAmounts[IndexID] <= 0)
                {
                    Console.WriteLine("Automatic repayment: Your loan has been fully repaid with this payment.");
                    UserHasActiveLoan[IndexID] = false;
                    UserLoanAmounts[IndexID] = 0;
                    UserLoanInterestRates[IndexID] = 0;
                }
                else
                {
                    Console.WriteLine("Automatic repayment: 150 OMR has been deducted toward your loan.");
                    Console.WriteLine($"Remaining loan balance: {UserLoanAmounts[IndexID]} OMR.");
                }
            }
            else
            {
                Console.WriteLine("Automatic repayment attempted, but insufficient balance to deduct 150 OMR for loan repayment.");
            }
        }

        // add salary to user balance in first every month 

        public static void AddSalaryToUsersMonthly()
        {
            double salaryAmount = 600.0;

            if (DateTime.Now.Day == 1)
            {
                for (int i = 0; i < UserBalances.Count; i++)
                {
                    // Ensure UserTransactions[i] exists
                    while (UserTransactions.Count <= i)
                    {
                        UserTransactions.Add(new List<string>());
                    }

                    // Add salary
                    UserBalances[i] += salaryAmount;
                    Console.WriteLine($"Salary of {salaryAmount} OMR has been added to {AccountUserNames[i]}'s account. New balance: {UserBalances[i]} OMR.");

                    // Log transaction
                    string transactionRecord = $"{DateTime.Now:yyyy-MM-dd},Salary,{salaryAmount},{UserBalances[i]},-";
                    UserTransactions[i].Add(transactionRecord);

                    // Auto-deduct loan payment if applicable
                    AutoRepayLoanIfApplicable(i);
                }

                Console.WriteLine("Monthly salaries have been deposited for all users.");
            }
            else
            {
                Console.WriteLine("Today is not the 1st of the month. Salaries were not deposited.");
            }
        }

        // View Last N Transactions function 
        public static void ViewLastNTransactions(int IndexID)
        {
            try
            {
                // Check if the user has any transactions
                if (UserTransactions.Count == 0 || UserTransactions[IndexID] == null || UserTransactions[IndexID].Count == 0)
                {
                    Console.WriteLine("No transactions found for this account.");
                    return;
                }
                // Ask user for the number of recent transactions to display

                Console.Write("Enter the number of recent transactions to display: ");
                if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
                {
                    Console.WriteLine("Invalid number entered.");
                    return;
                }

                if (IndexID < 0 || IndexID >= UserTransactions.Count)
                {
                    Console.WriteLine("Invalid user index.");
                    return;
                }

                // Retrieve the user's transactions
                var userTransactions = UserTransactions[IndexID]; // use var instead of use string where var is inferred as List<string> by the compiler. and it insted of use  List<string> userTransactions = UserTransactions[IndexID];
                // You do not use string because UserTransactions[IndexID] is not a single string, it is a List<string>.

                //Use:

                // List<string> userTransactions = UserTransactions[IndexID]; (explicit, clear for beginners).

                //var userTransactions = UserTransactions[IndexID]; (clean, when comfortable with type inference).

                // Using string here is conceptually and syntactically incorrect.


                if (userTransactions.Count == 0)
                {
                    Console.WriteLine("No transactions found for this account.");
                    return;
                }
                // Show the last n transactions for the user,
                int start = Math.Max(0, userTransactions.Count - n);

                Console.WriteLine($"Last {n} transactions for {AccountUserNationalID[IndexID]}:");
                for (int i = start; i < userTransactions.Count; i++)
                {
                    Console.WriteLine(userTransactions[i]);
                    UserFeedbackSystem("View Last Transactions");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        // View Transactions After a Date function
        public static void ViewTransactionsAfterDate(int IndexID)
        {
            Console.Write("Enter date (YYYY-MM-DD): ");
            string dateInput = Console.ReadLine();

            if (!DateTime.TryParse(dateInput, out DateTime filterDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            if (IndexID < 0 || IndexID >= UserTransactions.Count)
            {
                Console.WriteLine("Invalid user index.");
                return;
            }

            var userTransactions = UserTransactions[IndexID];

            bool found = false;
            Console.WriteLine($"Transactions after {filterDate:yyyy-MM-dd} for {AccountUserNames[IndexID]}:");

            foreach (var transaction in userTransactions)
            {
                var parts = transaction.Split(',');
                if (DateTime.TryParse(parts[0], out DateTime transactionDate))
                {
                    if (transactionDate > filterDate)
                    {
                        Console.WriteLine(transaction);
                        found = true;
                        UserFeedbackSystem("View Transactions After Date");
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("No transactions found after the specified date.");
            }
        }

        // User Feedback for Transection function 
        public static void UserFeedbackSystem(string TransectionType)
        {
            int Rate = 0;
            Console.WriteLine($"Please provide your rate service feedback for the {TransectionType} transaction from 1 to 5 (1: very bad, 5: Excellent):");
            Rate = int.Parse(Console.ReadLine());
            // Validate the rate input
            if (Rate < 1 || Rate > 5)
            {
                Console.WriteLine("Invalid rate. Please enter a number between 1 and 5.");
                return;
            }
            // Add the rate and transection type in one string to the UserFeedback list
            string Feedback = TransectionType +"|" + Rate;

            UserFeedbacks.Add(Feedback);
            Console.WriteLine($"Thank you for your feedback! You rated the {TransectionType} transaction as {Rate} out of 5.");
            SaveUserFeedbackToFile();
        }

        // Display All User Transaction 
        public static void PrintAllTransactions(int IndexID)
        {
            
            LoadUserTransactionsFromFile();
            string[] parts = new string[0]; // Initialize parts to avoid null reference
            bool isfoundTransaction = false;
            //Read each line until the end of the file
            for (int i=0; i< UserTransactions.Count; i++)
            {
                parts = UserTransactions[i][0].Split(','); // Split the first transaction line to get the account number
                if (parts.Length > 0 && parts[0] == AccountUserNationalID[IndexID])
                {
                    isfoundTransaction = true;
                }
            }
            if (!isfoundTransaction)
            {
                Console.WriteLine("No transactions found for this user.");
                return;
            }
            else
            {
                Console.WriteLine($"All Transactions for {AccountUserNames[IndexID]} (Account: {AccountNumbers[IndexID]}):");
                Console.WriteLine("Date         | Type      | Amount   | Balance After");
                Console.WriteLine("---------------------------------------------------");
                // Loop through the user's transactions and print them
                foreach (var transaction in UserTransactions[IndexID])
                {
                    parts = transaction.Split(',');
                    if (parts.Length >= 4)
                    {
                        string date = parts[0];
                        string type = parts[1];
                        string amount = parts[2];
                        string balanceAfter = parts[3];
                        Console.WriteLine($"{date,-12} | {type,-9} | {amount,-8} | {balanceAfter}");
                    }
                }
            }

        }

        // Request Book Appointment Function
        public static void RequestBookAppointment(int IndexID)
        {
            LoadAppointmentRequestsFromFile();

            // Check if user already has an appointment in the queue
            bool hasExistingAppointment = false;
            foreach (string item in AppointmentRequests)
            {
                string[] parts = item.Split('|');
                if (parts.Length >= 4 && parts[1] == AccountUserNationalID[IndexID])
                {
                    hasExistingAppointment = true;
                    Console.WriteLine($"You already have an appointment for {parts[2]} on {parts[3]}.");
                    Console.WriteLine("Do you want to cancel this appointment? (yes/no)");
                    string response = Console.ReadLine().ToLower();

                    if (response == "yes")
                    {
                        // Create a temporary queue excluding this appointment
                        Queue<string> tempQueue = new Queue<string>();
                        while (AppointmentRequests.Count > 0)
                        {
                            string currentItem = AppointmentRequests.Dequeue();
                            string[] currentParts = currentItem.Split('|');
                            if (!(currentParts.Length >= 4 && currentParts[1] == AccountUserNationalID[IndexID]))
                            {
                                tempQueue.Enqueue(currentItem);
                            }
                        }
                        AppointmentRequests = tempQueue;

                        UserHasActiveAppointment[IndexID] = false;
                        UserAppointmentDates[IndexID] = DateTime.MinValue;
                        Console.WriteLine("Your appointment has been cancelled.");

                        SaveAppointmentRequestsToFile();

                        Console.WriteLine("Do you want to book a new appointment? (yes/no)");
                        response = Console.ReadLine().ToLower();
                        if (response != "yes")
                        {
                            Console.WriteLine("Appointment booking cancelled.");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Proceeding to book a new appointment...");
                        }
                    }
                    else
                    {
                        Console.WriteLine("You chose not to cancel your existing appointment.");
                        return;
                    }
                    break;
                }
            }

            if (!hasExistingAppointment)
            {
                Console.WriteLine("You do not have an existing appointment.");
                Console.WriteLine("Do you want to book a new appointment? (yes/no)");
                string response = Console.ReadLine().ToLower();
                if (response != "yes")
                {
                    Console.WriteLine("Appointment booking cancelled.");
                    return;
                }
                else
                {
                    Console.WriteLine("Select Service Type:\n1. Loan Discussion\n2. Account Consultation");
                    string choice = Console.ReadLine();
                    string serviceType = choice == "1" ? "Loan Discussion" : choice == "2" ? "Account Consultation" : "";

                    if (string.IsNullOrEmpty(serviceType))
                    {
                        Console.WriteLine("Invalid choice.");
                        return;
                    }

                    Console.Write("Enter desired appointment date and time (YYYY-MM-DD HH:MM): ");
                    string input = Console.ReadLine();

                    if (!DateTime.TryParse(input, out DateTime appointmentDateTime))
                    {
                        Console.WriteLine("Invalid date/time format.");
                        return;
                    }

                    string AppointmentRequest = IndexID + "|" + AccountUserNationalID[IndexID] + "|" + serviceType + "|" + appointmentDateTime.ToString("yyyy-MM-dd HH:mm");
                    AppointmentRequests.Enqueue(AppointmentRequest);
                    UserHasActiveAppointment[IndexID] = true;
                    UserAppointmentDates[IndexID] = appointmentDateTime;

                    Console.WriteLine($"Appointment for {serviceType} booked successfully on {appointmentDateTime}.");
                    SaveAppointmentRequestsToFile();
                }
            }

            
        }




        // ===================== Admin Features Function ==========================
        // Admin create account 
        public static void AdminCreateAccount()
        {
            string UserName = "";
            bool ValidName = true;
            string UserID = "";
            bool ValidID = false;
            bool IsSave = true;
            int tries = 0;
            string password = "";
            string hashedPassword = "";
            // Error handling 
            try
            {
                // 1. Set User Name Process
                do
                {
                    // ask user to enter his name
                    Console.WriteLine("Enter Your Name: ");
                    string name = Console.ReadLine();
                    // valid the name input 
                    ValidName = stringOnlyLetterValidation(name);
                    if (ValidName == true)
                    {
                        UserName = name;
                        IsSave = true;
                    }
                    else
                    {
                        IsSave = false;
                        tries++;
                    }
                    
                } while (ValidName == false && tries < 3);
                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    return;
                }
                tries = 0;
                // 2. Set User ID Process
                do
                {
                    // ask user to enter his national ID 
                    Console.WriteLine("Enter your National ID: ");
                    string ID = Console.ReadLine();
                    // valid the ID input
                    ValidID = IDValidation(ID);
                    // check if 
                    if (ValidID == true)
                    {
                        UserID = ID;
                        IsSave = true;
                    }
                    else
                    {
                        IsSave = false;
                        tries++;
                    }
                   
                } while (ValidID == false && tries < 3);
                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    return;
                }
                tries = 0;
                // 3. Set Password with masking + hashing
                do
                {
                    Console.Write("Set your password: ");
                    password = ReadPassword(); // masked input
                    hashedPassword = HashPassword(password);
                    if (string.IsNullOrEmpty(hashedPassword))
                    {
                        Console.WriteLine("Password cannot be empty. Please try again.");
                        tries++;
                    }
                    else
                    {
                        IsSave = true;
                    }
                } while (IsSave == false); // Ensure password is not empty

                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    Console.ReadLine();
                    return;
                }
                // reset tries to 0 for next validation
                tries = 0;

                // Save information in the Admin lists 
                if (IsSave == true)
                {
                    bool AlreadyExist = false;
                    // loop in queue to chech if request with same id already submit to Prevent Duplicate Account Requests 
                    for(int i = 0; i < AdminID.Count; i++) {
                        //check if id in rqures queue id exist or not 
                        if (AdminID[i] == UserID)
                        {
                            // if yes put AlreadyRequested flag with true value
                            AlreadyExist = true;
                            break;
                        }

                    }
                    // based on AlreadyRequested flad we decided if we save user inputes of account information or not 
                    if (AlreadyExist)
                    {
                        Console.WriteLine("Admin with this ID number is already exist");
                    }
                    else
                    {
                        int NewAdminAccountNumber = LastAdminAccountNumber + 1;
                        AdminAccountNumber.Add(NewAdminAccountNumber);
                        AdminName.Add(UserName);
                        AdminID.Add(UserID);
                        AccountAdminHashedPasswords.Add(hashedPassword);
                        Console.WriteLine("Request Account Creation successfully submitted.");
                        LastAdminAccountNumber = NewAdminAccountNumber;
                    }
                }


            }
            catch
            {
                // display message submit failed 
                Console.WriteLine("Admin Account Creation failed");
            }

        }
        // Admin Operation 
        public static void AdminMenuOperations()
        {
            bool InAdminMenu = true;
            // while loop to display the mnue ewhile the flag is true
            while (InAdminMenu)
            {
                // display All Admin Menu
                Console.Clear();
                Console.WriteLine("\n------ Admin Menu ------");
                Console.WriteLine("1. Process Next Account Request");
                Console.WriteLine("2. View Submitted Reviews");
                Console.WriteLine("3. View All Accounts");
                Console.WriteLine("4. View Pending Account Requests");
                Console.WriteLine("5. Search User account by user National ID");
                Console.WriteLine("6. Show Total Bank Balance");
                Console.WriteLine("7. Delete Account");
                Console.WriteLine("8. Show Top 3 Richest Customers");
                Console.WriteLine("9. Export All Account Info to a New File (CSV or txt)");
                Console.WriteLine("10. Process Loan Requests");
                Console.WriteLine("11. Average Of FeedBack Rate");
                Console.WriteLine("12. View User Transaction");
                Console.WriteLine("13. View and Process Appointment Requests");
                Console.WriteLine("14. Unlock User Account");
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select option: ");
                string adminChoice = Console.ReadLine().Trim(); // Read user input from console
                Console.WriteLine();

                // use switch to select one of many code blocks to be executed
                switch (adminChoice)
                {
                    // case to Process Next Account Request
                    case "1":
                        ProcessAccountRequest();
                        Console.ReadLine();
                        break;
                    // case to View Submitted Reviews
                    case "2":
                        ViewReviews();
                        Console.ReadLine();
                        break;
                    // case to View All Accounts
                    case "3":
                        ViewAllAccounts();
                        Console.ReadLine();
                        break;
                    // case to View Pending Account Requests
                    case "4":
                        ViewPendingRequests();
                        Console.ReadLine();
                        break;
                    // Search user by enter user National ID
                    case "5":
                        int UserIndexID = EnterUserID();
                        SearchUserByNationalID(UserIndexID);
                        Console.ReadLine();
                        break;
                    // Show Total Bank Balance
                    case "6":
                        ShowTotalBankBalance();
                        Console.ReadLine();
                        break;
                    // Delete Account
                    case "7":
                        int deleteIndexID = EnterNationalID();
                        if (deleteIndexID != -1)
                        {
                            DeleteAccount(deleteIndexID);
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        Console.ReadLine();
                        break;
                    case "8":
                        ShowTop3RichestCustomers();
                        Console.ReadLine();
                        break;
                    // Export All Account Info to a New File (CSV or txt)
                    case "9":
                        ExportAccountsToFile(ExportFilePath);
                        Console.WriteLine($"All account information has been exported to {ExportFilePath}");
                        Console.ReadLine();
                        break;
                    // case to Process Loan Requests
                    case "10":
                        ProcessLoanRequests();
                        Console.ReadLine();
                        break;

                    // case to Average Of Transaction Rate
                    case "11":
                        Console.WriteLine("Calculating average transaction rate...");
                        // Ask to enter the type of tansaction for which to calculate the average rate
                        Console.WriteLine("Enter the type of transaction:");
                        Console.WriteLine("1. Request Creation Account");
                        Console.WriteLine("2. Updated Phone");
                        Console.WriteLine("3. Updated Address");
                        Console.WriteLine("4. Deposit");
                        Console.WriteLine("5. Withdraw");
                        Console.WriteLine("6. Submit Review");
                        Console.WriteLine("7. Transfer");
                        Console.WriteLine("8. Generate Monthly Statement");
                        Console.WriteLine("9. Request Loan");
                        Console.WriteLine("10. View Last Transactions");
                        Console.WriteLine("11. View Transactions After Date");
                        Console.WriteLine("0. Return to Admin Menu");

                        string transactionTypeChoice = Console.ReadLine();
                        string transactionType = "";
                        double averageRate = 0.0; // Initialize average rate variable
                        switch (transactionTypeChoice)

                        {
                            case "1":
                                transactionType = "Request Creation Account";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "2":
                                transactionType = "Updated Phone";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "3":
                                transactionType = "Updated Address";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "4":
                                transactionType = "Deposit";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "5":
                                transactionType = "Withdraw";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "6":
                                transactionType = "Submit Review";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "7":
                                transactionType = "Transfer";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "8":
                                transactionType = "Generate Monthly Statement";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "9":
                                transactionType = "Request Loan";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "10":
                                transactionType = "View Last Transactions";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            case "11":
                                transactionType = "View Transactions After Date";

                                // Call the function to calculate the average transaction rate
                                averageRate = CalculateAverageFeedback(transactionType);
                                Console.WriteLine($"The average transaction rate is: {averageRate}");
                                Console.ReadLine();

                                break;
                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                continue; // Skip to the next iteration of the loop
                        }
                        break;

                    // case to View User Transaction
                    case "12":
                        // Ask the admin to enter the user National ID
                        int userIndexID = EnterUserID();
                        if (userIndexID != -1)
                        {
                            PrintAllTransactions(userIndexID);
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        Console.ReadLine();
                        break;
                    // case to View and Process Appointment Requests
                    case "13":
                        ProcessRequestAppointments();
                        Console.ReadLine();
                        break;

                    // case to Unlock User Account
                    case "14":
                        UnlockUserAccount();
                        Console.ReadLine();
                        break;
                    // case to Return to Main Menu
                    case "0":
                        InAdminMenu = false; // this will exit the loop and return
                        break;
                    // default case to display message to the admin if selected the wronge number
                    default:
                        Console.WriteLine("Wronge choice number, Try Agine!");
                        Console.ReadKey();
                        break;

                }
            }

        }
        // login Admin with ID and password
        public static int AdminLoginWith_ID_Password()
        {
            int FoundIndex = -1;
            int IndexId = -1;
            IndexId = EnterAdminID();
            if (IndexId == -1)
            {
                FoundIndex  = - 1;
            }
            else
            {
                IndexId = EnterAdminPassword(IndexId);
                if (IndexId == -1)
                {
                    FoundIndex = -1;
                }

                else
                {
                    FoundIndex = IndexId;
                }

            }         

            return IndexId;
        }
        // View Pending Requests Function 
        public static void ViewPendingRequests()
        {
            //check if tere is any pending requests
            if (createAccountRequests.Count() == 0)
            {
                Console.WriteLine("There is no pending request yet");
            }
            else
            {
                // display all pending request
                foreach (string request in createAccountRequests)
                {
                    Console.WriteLine(request);
                    Console.WriteLine("=======================");
                }
            }
        }
        // View All Accounts Function 
        public static void ViewAllAccounts()
        {
            
            //iteration for loop all index values in lists
            Console.WriteLine($"Account ID | User Name | National ID | Balance | Phone | Address");

            for (int i = 0; i < AccountUserNationalID.Count; i++)
            {
                //display list values for every index
                Console.WriteLine($"{AccountNumbers[i]} | {AccountUserNames[i]} | {AccountUserNationalID[i]} | {UserBalances[i]} | {UserPhoneNumbers[i]} | {UserAddresses[i]}");
            }

        }
        // View Reviews Function 
        public static void ViewReviews()
        {
            //error handling using try-catch
            try
            {
                //iteration all users reviews in UserReviews stack 
                foreach (string Review in UserReviewsStack)
                {
                    Console.WriteLine(Review);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        // Process Account Request Function 
        public static void ProcessAccountRequest()
        {
            // handling error using try-catch
            try
            {
                // check if there is request
                if (createAccountRequests.Count == 0)
                {
                    Console.WriteLine("No pending account requests.");
                    return;
                }
                // get last element (which it is first element enter) in the queue
                string request = createAccountRequests.Dequeue();
                // Split the request string using '|' to separate username and national ID
                string[] SplitRrquest = request.Split("|");
                // Extract and store the username from the request
                string UserName = SplitRrquest[0];
                Console.WriteLine($"User Name: {UserName} ");
                // Extract and store the national ID from the request
                string UserNationalID = SplitRrquest[1];
                Console.WriteLine($"User National ID: {UserNationalID} ");
                // Extract the hashed password from the request
                string UserHashedPassword = SplitRrquest[2];
                // Extract the phone number from the request
                string UserPhoneNumber = SplitRrquest[3];
                // Extract the address from the request
                string UserAddress = SplitRrquest[4];
                // Increment the last account ID number for the new account
                int NewAccountIDNumber = LastAccountNumber + 1;
                // Set initial account balance to 0
                double balance = MinimumBalance;
                Console.WriteLine("Do you want to accept account creation request (y/n) !");
                char choice = Console.ReadKey().KeyChar;
                if (choice == 'y' || choice == 'Y')
                {
                    // Add user name in the AccountUserNames list
                    AccountUserNames.Add(UserName);
                    // Add user national ID in the AccountUserNationalID list
                    AccountUserNationalID.Add(UserNationalID);
                    // Add user Account ID in the AccountIDNumbers list
                    AccountNumbers.Add(NewAccountIDNumber);
                    // Add user initial balance in the Balances list
                    UserBalances.Add(balance);
                    // Add user hashed password in the AccountUserHashedPasswords list
                    AccountUserHashedPasswords.Add(UserHashedPassword);
                    // Add user phone number in the UserPhoneNumbers list
                    UserPhoneNumbers.Add(SplitRrquest[3]);
                    // Add user address in the UserAddresses list
                    UserAddresses.Add(SplitRrquest[4]);
                    // Add user has active loan in the UserHasActiveLoan list
                    UserHasActiveLoan.Add(false);
                    // Add user loan amounts in the UserLoanAmounts list
                    UserLoanAmounts.Add(0);
                    // Add user interest rates in the UserLoanInterestRates list
                    UserLoanInterestRates.Add(0);
                    // Add user has active appointment in the UserHasActiveAppointment list
                    UserHasActiveAppointment.Add(false);
                    // Add user appointment dates in the UserAppointmentDates list
                    UserAppointmentDates.Add(DateTime.MinValue);
                    // Add user transactions in the UserTransactions list
                    UserIsLocked.Add(false);

                    Console.WriteLine($"Account created for {UserName} with Account Number: {NewAccountIDNumber}, Phone Number: {UserPhoneNumber} and address: {UserAddress}");
                    // display message to the user that account created successfully
                    Console.WriteLine("Account Accepted successfully.");
                    //Since a new user was added,
                    
                    LastAccountNumber = NewAccountIDNumber;

                    



                }
                else
                {
                    Console.WriteLine("Account Dose not accept!");
                    string InAcceptRequest = request;
                    CancelcreateAccountRequests.Enqueue(InAcceptRequest);
                }

            }
            catch
            {
                //display massage to the user if anyy error happened during running program 
                Console.WriteLine("Accept process fail, Try Agine!");
            }
            
        }

        //  Search by National ID or Name (Admin Tool)
        public static void SearchUserByNationalID(int UserIndexID)
        {
            
            //display user account number, name , balance with the enter national id 
            Console.WriteLine($"User Account Numbaer : {AccountNumbers[UserIndexID]}");
            Console.WriteLine($"User Name : {AccountUserNames[UserIndexID]}");
            Console.WriteLine($"User Balance : {UserBalances[UserIndexID]}");
            

        }
        //  Show Total Bank Balance
        public static void ShowTotalBankBalance()
        {
            // Calculate the total balance by summing all user balances
            double totalBalance = UserBalances.Sum();
            // Display the total balance
            Console.WriteLine($"Total Bank Balance: {totalBalance}");
        }

        // Delete Account
        public static void DeleteAccount(int IndexID)
        {
            // Check if the user exists in the list
            if (IndexID >= 0 && IndexID < AccountUserNationalID.Count)
            {
                // Remove the user from all lists
                AccountUserNames.RemoveAt(IndexID);
                AccountUserNationalID.RemoveAt(IndexID);
                UserBalances.RemoveAt(IndexID);
                AccountNumbers.RemoveAt(IndexID);
                Console.WriteLine("Account deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid user index. Account deletion failed.");
            }
        }

        // Show Top 3 Richest Customers 
        public static void ShowTop3RichestCustomers()
        {
            //// Check if there are at least 3 users
            //if (UserBalances.Count < 3)
            //{
            //    Console.WriteLine("Not enough users to determine the top 3 richest customers.");
            //    return;
            //}
            //// Create a list of tuples containing user index and balance
            //var userBalancesWithIndex = UserBalances.Select((balance, index) => (index, balance)).ToList();
            //// Sort the list by balance in descending order
            //var top3Richest = userBalancesWithIndex.OrderByDescending(x => x.balance).Take(3).ToList();
            //// Display the top 3 richest customers
            //Console.WriteLine("Top 3 Richest Customers:");
            //foreach (var user in top3Richest)
            //{
            //    Console.WriteLine($"Account Number: {AccountNumbers[user.index]}, Name: {AccountUserNames[user.index]}, Balance: {user.balance}");
            //}

            var top3Richest = AccountNumbers
                .Select((accNum, index) => new
                {
                    AccountNumber = accNum,
                    Name = AccountUserNames[index],
                    Balance = UserBalances[index]
                })
                .OrderByDescending(x => x.Balance)
                .Take(3);

            foreach (var user in top3Richest)
            {
                Console.WriteLine($"{user.AccountNumber} | {user.Name} | {user.Balance}");
            }

        }
        /* 
         Export All Account Info to a New File (CSV or txt) 
            • Create a clean export with headers and all customer info.
        */

        public static void ExportAccountsToFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write headers
                    writer.WriteLine("Account Number,User Name,National ID,Balance");
                    // Iterate through all accounts and write their information
                    for (int i = 0; i < AccountUserNationalID.Count; i++)
                    {
                        writer.WriteLine($"{AccountNumbers[i]},{AccountUserNames[i]},{AccountUserNationalID[i]},{UserBalances[i]}");
                    }
                }
                Console.WriteLine($"Accounts exported successfully to {filePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error exporting accounts: {e.Message}");
            }
        }

        // Process Loan Requests
        public static void ProcessLoanRequests()
        {
            if (LoanRequests.Count == 0)
            {
                Console.WriteLine("No pending loan requests.");
                return;
            }

            int initialCount = LoanRequests.Count;

            while (LoanRequests.Count > 0)
            {
                string request = LoanRequests.Dequeue();
                string[] parts = request.Split('|');
                int userIndex = int.Parse(parts[0]);
                double loanAmount = double.Parse(parts[1]);
                double interestRate = 0.30;

                Console.WriteLine($"Loan Request for User: {AccountUserNames[userIndex]} (Acc#: {AccountNumbers[userIndex]})");
                Console.WriteLine($"Requested Amount: {loanAmount}, Interest Rate: 30%");
                Console.Write("Approve this loan? (y/n): ");
                string choice = Console.ReadLine();

                if (choice.ToLower() == "y")
                {
                    UserBalances[userIndex] += loanAmount;
                    UserHasActiveLoan[userIndex] = true;
                    UserLoanAmounts[userIndex] = loanAmount;
                    UserLoanInterestRates[userIndex] = interestRate;

                    Console.WriteLine($"Loan approved and {loanAmount} added to user balance.");
                }
                else
                {
                    Console.WriteLine("Loan request rejected.");
                }
            }

            Console.WriteLine($"{initialCount} loan requests processed.");
        }

        // Calculate the average feedback rating for a specific transaction type
        public static double CalculateAverageFeedback(string transactionType)
        {
            LoadUserTransactionsFromFile();
            if (string.IsNullOrEmpty(transactionType))
            {
                Console.WriteLine("Transaction type cannot be null or empty.");
                return 0;
            }
            var feedbacks = UserFeedbacks.Where(f => f.StartsWith(transactionType + "|")).Select(f => f.Split('|')[1]).Select(int.Parse).ToList();
            if (feedbacks.Count == 0)
                return 0;
            return feedbacks.Average();
        }

        // View and Process Appointment Requests
        /*public static void ProcessRequestAppointments()
          {
              LoadAppointmentRequestsFromFile();

              if (AppointmentRequests.Count == 0)
              {
                  Console.WriteLine("No appointment requests to process.");
                  return;
              }

              int initialCount = AppointmentRequests.Count;
              Queue<string> tempQueue = new Queue<string>();

              while (AppointmentRequests.Count > 0)
              {
                  string request = AppointmentRequests.Dequeue();
                  string[] parts = request.Split('|');
                  if (parts.Length < 4)
                      continue;

                  int userIndex = int.Parse(parts[0]);
                  string userNationalID = parts[1];
                  string serviceType = parts[2];
                  DateTime appointmentDateTime = DateTime.Parse(parts[3]);

                  Console.WriteLine($"Appointment Request for {AccountUserNames[userIndex]} (Acc#: {AccountNumbers[userIndex]}):");
                  Console.WriteLine($"Service: {serviceType}, Date/Time: {appointmentDateTime}");
                  Console.Write("Mark appointment as completed? (y/n): ");
                  string choice = Console.ReadLine().ToLower();

                  if (choice == "y")
                  {
                      UserHasActiveAppointment[userIndex] = false;
                      UserAppointmentDates[userIndex] = DateTime.MinValue;
                      Console.WriteLine("Appointment marked as completed.");

                      Console.WriteLine($"{initialCount} appointment requests processed.");
                  }
                  else
                  {
                      tempQueue.Enqueue(request);
                      Console.WriteLine("Appointment retained for later processing.");
                  }
              }


          }*/
        public static void ProcessRequestAppointments()
        {
            LoadAppointmentRequestsFromFile();

            if (AppointmentRequests.Count == 0)
            {
                Console.WriteLine("No appointment requests to process.");
                return;
            }

            Console.WriteLine("Filter appointments by service type:");
            Console.WriteLine("1. Loan Discussion");
            Console.WriteLine("2. Account Consultation");
            Console.Write("Select option (or press Enter to process all): ");
            string filterChoice = Console.ReadLine().Trim();
            string serviceFilter = filterChoice switch
            {
                "1" => "Loan Discussion",
                "2" => "Account Consultation",
                _ => null  // No filter
            };

            // Convert the queue to a list and apply filtering via LINQ
            var filteredAppointments = AppointmentRequests
                .Where(req =>
                {
                    var parts = req.Split('|');
                    return parts.Length >= 4 && (serviceFilter == null || parts[2] == serviceFilter);
                })
                .ToList();

            if (filteredAppointments.Count == 0)
            {
                Console.WriteLine("No matching appointment requests found.");
                return;
            }

            Queue<string> tempQueue = new Queue<string>();

            foreach (var request in AppointmentRequests)
            {
                string[] parts = request.Split('|');
                if (parts.Length < 4)
                    continue;

                int userIndex = int.Parse(parts[0]);
                string userNationalID = parts[1];
                string serviceType = parts[2];
                DateTime appointmentDateTime = DateTime.Parse(parts[3]);

                // Skip if filtering and this item doesn't match
                if (serviceFilter != null && serviceType != serviceFilter)
                {
                    tempQueue.Enqueue(request);
                    continue;
                }

                Console.WriteLine($"\nAppointment Request for {AccountUserNames[userIndex]} (Acc#: {AccountNumbers[userIndex]}):");
                Console.WriteLine($"Service: {serviceType}, Date/Time: {appointmentDateTime}");
                Console.Write("Mark appointment as completed? (y/n): ");
                string choice = Console.ReadLine().ToLower();

                if (choice == "y")
                {
                    UserHasActiveAppointment[userIndex] = false;
                    UserAppointmentDates[userIndex] = DateTime.MinValue;
                    Console.WriteLine("Appointment marked as completed.");
                }
                else
                {
                    tempQueue.Enqueue(request);
                    Console.WriteLine("Appointment retained for later processing.");
                }
            }

            AppointmentRequests = tempQueue;
            SaveAppointmentRequestsToFile();
        }



        // Unlock User Account
        public static void UnlockUserAccount()
        {
            Console.Write("Enter National ID of the user to unlock: ");
            string enteredID = Console.ReadLine();

            int IndexID = AccountUserNationalID.IndexOf(enteredID);
            if (IndexID == -1)
            {
                Console.WriteLine("User with this National ID does not exist.");
                return;
            }

            if (!UserIsLocked[IndexID])
            {
                Console.WriteLine("This account is not locked.");
                return;
            }

            UserIsLocked[IndexID] = false;
            Console.WriteLine($"Account for {AccountUserNames[IndexID]} has been unlocked successfully.");
        }


        // ************************************************* String Validation **********************************************
        public static bool stringOnlyLetterValidation(string word)
        {
            bool IsValid = true;
            string ValidWord="";
            if (string.IsNullOrEmpty(word) && word.All(char.IsLetter))
            {
                Console.WriteLine("Input is just empty!");
                IsValid = false;
  
            }
            else
            {
                IsValid = true;
            }

            if (Regex.IsMatch(word, @"^[a-zA-Z]+$"))
            {
                //Console.WriteLine("Valid: only letters.");
                IsValid = true;
            }
            else
            {
                Console.WriteLine("Invalid: contains non-letter characters.");
                IsValid = false;
            }

            return IsValid;
        }
        public static bool StringlettersWithNumbers(string word)
        {
            bool IsValid = true;
            string ValidWord = "";
            if (string.IsNullOrEmpty(word) && word.All(char.IsLetter))
            {
                Console.WriteLine("Input is just empty!");
                IsValid = false;

            }
            else
            {
                IsValid = true;
            }

            return IsValid;
        }

        //******************************************** Phone number Validation ******************************************
        public static bool IsValidPhoneNumber(string phoneInput)
        {
            if (string.IsNullOrWhiteSpace(phoneInput))
            {
                Console.WriteLine("Phone number cannot be empty.");
                return false;
            }
            if (!phoneInput.All(char.IsDigit))
            {
                Console.WriteLine("Phone number must contain digits only.");
                return false;
            }
            if (phoneInput.Length != 8)
            {
                Console.WriteLine("Phone number must be exactly 8 digits.");
                return false;
            }
            return true;
        }


        // ===================================== User National ID =============================
        public static int EnterNationalID()
        {
            int IdIndex = 0;
            Console.WriteLine("Enter your National ID: ");
            string NationalID = Console.ReadLine();

            bool IsExist = CheckUserIDExist(NationalID);
            if (IsExist)
            {
                // If the ID exists, return the index of the user
                for (int i = 0; i < AccountUserNationalID.Count; i++)
                {
                    if (AccountUserNationalID[i] == NationalID)
                    {
                        IdIndex = i; 
                    }
                }

            }
            else
            {
                Console.WriteLine("User with this ID does not exist. Please try again.");
                return -1; // Return -1 to indicate that the user does not exist
            }
            return IdIndex;
        }
        //NationalID validation formate
        public static bool IDValidation(string NationalID)
        {
            bool IsValid = true;
            // Check if the input is not null or empty
            if (!string.IsNullOrWhiteSpace(NationalID))
            {
               
                if (Regex.IsMatch(NationalID, @"^\d+$"))
                {
                    // Check if input is exactly 8 digits and only contains numbers
                    if (NationalID.Length == 8 && NationalID.All(char.IsDigit))
                    {
                        //Console.WriteLine("your National ID : " + NationalID);
                        IsValid = true;
                    }
                    else
                    {
                        IsValid = false;
                    }

                }
                else
                {
                    IsValid = false;
                }
               
            }
            else
            {
                IsValid = false;
            }

            return IsValid;
        }

        // Check the exist of user ID in the list
        public static bool CheckUserIDExist(string UserID)
        {

            // Loop through the list of registered National IDs
            //for (int i = 0; i < AccountUserNationalID.Count; i++)
            //{
            //    // Check if the current ID in the list matches the user's input
            //    if (AccountUserNationalID[i] == UserID)
            //    {
            //        return true; // User ID exists in the list
            //    }
            //}
            //return false; // User ID does not exist in the list

            ///////////////////////// Use Linq in search////////////////
            var user = AccountUserNationalID
                .Select((id, index) => new { Index = index, ID = id })
                .FirstOrDefault(x => x.ID == UserID);

            if (user != null)
            {
                //int index = user.Index;
                return true;
            }

            else
            {
                return false; // User ID does not exist in the list
            }
        }

        // numeric validation with double value
        public static bool AmountValid(string amount)
        {
            // Define a regular expression pattern to match decimal numbers (e.g., 10.5)
            string pattern = @"^\d+\.\d+$";
            // Check if the input string is not null or empty
            if (!string.IsNullOrEmpty(amount))
            {
                // Check if the input string matches the decimal format defined by the regex
                if (Regex.IsMatch(amount, pattern))
                {
                    // Try converting the string to a double
                    if (double.TryParse(amount, out double result))
                    {
                        // Check if the parsed number is greater than zero
                        if (result > 0)
                        {
                            // Input is valid, print confirmation and return true
                            //Console.WriteLine("Valid input: " + result);
                            return true;

                        }
                        else
                        {
                            // Number is less than or equal to zero
                            Console.WriteLine("Amount must be greater than zero.");
                        }
                    }
                    else
                    {
                        // Parsing failed despite matching the regex (edge case)
                        Console.WriteLine("Invalid format. Please enter a valid number (e.g., 0.0)");
                    }
                }
                else
                {
                    // Input doesn't match the required decimal format
                    Console.WriteLine("Invalid format. Please enter a number with valid formate (0.0)");
                }
            }
            else
            {
                // Input is null or empty
                Console.WriteLine("Invalid null or empty value! Try again.");
            }
            // Return false if any validation step fails
            return false;
        }

        //*************************** Ask to enter ID number *************************
        public static int IDAdminExist(string ID)
        {
            int AdminIndex = -1;

            // Start of try block to catch any unexpected runtime exceptions
            try
            {

                bool ValidID = IDValidation(ID); // Validate the input ID using a validation method
                if (ValidID == true)  // Proceed only if the ID is valid
                {
                    bool userFound = false;
                    // Loop through the list of registered National IDs
                    for (int i = 0; i < AdminID.Count; i++)
                    {
                        // Check if the current ID in the list matches the user's input
                        if (AdminID[i] == ID)
                        {
                            AdminIndex = i;
                            userFound = true;  // If match found, set userFound = true
                            break;
                        }
                    }
                    if (userFound == false)
                    { 
                        // If loop completes with no match, show message
                        Console.WriteLine("This Admin ID dose not exist");

                    }
                }
                else
                {
                    Console.WriteLine("National ID is invalid! please try agine");
                    Console.WriteLine("National ID should be exactly 8 digits and numeric only.");
                }

            }
            catch (Exception e) // Catch any exceptions that occur
            {
                Console.WriteLine(e.Message); // Print the error message
                AdminIndex = -1;
            }
            //Console.WriteLine($"UserLogin result: {ValidUserLogin}"); // Print the result of UserLogin for debugging
            return AdminIndex;

        }

        
        // ************************* saved files and loaded User and Admin Account Information  ****************************
        public static void SaveAccountsInformationToFile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(AccountsFilePath))
                {
                    // Loop through all accounts by index
                    for (int i = 0; i < AccountNumbers.Count; i++)
                    {

                        // Create a line of data combining account info separated by commas
                        string dataLine = $"{AccountNumbers[i]},{AccountUserNames[i]},{AccountUserNationalID[i]},{UserBalances[i]},{AccountUserHashedPasswords[i]},{UserPhoneNumbers[i]},{UserAddresses[i]}, {UserHasActiveLoan[i]},{UserLoanAmounts[i]}, {UserLoanInterestRates[i]}, {UserIsLocked[i]}, {UserHasActiveAppointment[i]},{UserAppointmentDates[i]}";// use Trim() with AccountUserHashedPasswords[i] to remove any extra spaces
                        //Console.WriteLine(dataLine);
                        // Write the data line into the file
                        writer.WriteLine(dataLine);

                        

                    }
                }
                // Inform the user that accounts were saved successfully
                Console.WriteLine("Accounts saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving file.");
            }
        }
        public static void LoadAccountsInformationFromFile()
        {
            try  // Try to execute the code inside the block
            {
                // Check if the accounts file does not exist
                if (!File.Exists(AccountsFilePath))
                {
                    // Inform the user that no data was found
                    Console.WriteLine("No saved data found.");
                    // Exit the method early
                    return;
                }
                // Clear the list of account numbers
                AccountNumbers.Clear();
                // Clear the list of account usernames
                AccountUserNames.Clear();
                // Clear the list of account usernames
                AccountUserNationalID.Clear();
                // Clear the list of user balances
                UserBalances.Clear();
                // Clear the list of account hashed passwords
                AccountUserHashedPasswords.Clear();
                // Clear the list of user phone numbers
                UserPhoneNumbers.Clear();
                // Clear the list of user addresses
                UserAddresses.Clear();
                // Clear the list of User Active Loan 
                UserHasActiveLoan.Clear();
                // Clear the list of user loan amount 
                UserLoanAmounts.Clear();
                // Clear the list of User loan interest rate 
                UserLoanInterestRates.Clear();
                // Clear the list of User Is Locked 
                UserIsLocked.Clear();
                //clear the list of User Has Active Appointment
                UserHasActiveAppointment.Clear();
                //clear the list of User Appointment Dates
                UserAppointmentDates.Clear();

                // Clear the list of transactions
                //transactions.Clear();

                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(AccountsFilePath))
                {
                    string line; // Declare a variable to hold each line
                                 // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Split the line into parts separated by commas
                        string[] parts = line.Split(',');
                        // Convert the first part to an integer
                        int accNum = Convert.ToInt32(parts[0]);
                        // Add the account number to the list
                        AccountNumbers.Add(accNum);
                        // Add the account username to the list
                        AccountUserNames.Add(parts[1]);
                        // Add the account user national ID to the list
                        AccountUserNationalID.Add(parts[2]);
                        // Convert the balance to double and add it to the list
                        UserBalances.Add(Convert.ToDouble(parts[3]));
                        // Add the account hashed password to the list
                        AccountUserHashedPasswords.Add(parts[4].Trim()); // use Trim() to remove any extra spaces
                        // Add the user phone number to the list
                        UserPhoneNumbers.Add(parts[5]);
                        // Add the user address to the list
                        UserAddresses.Add(parts[6]);
                        // Add the User active loan to list 
                        UserHasActiveLoan.Add(Convert.ToBoolean(parts[7]));
                        // Add the user Loa amount to the list 
                        UserLoanAmounts.Add(Convert.ToDouble(parts[8]));
                        // Add the user Loan Interest rate to list
                        UserLoanInterestRates.Add(Convert.ToDouble(parts[9]));
                        // Add the Account user locked to list
                        UserIsLocked.Add(Convert.ToBoolean(parts[10]));
                        // Add the User Has Active Appointment to list
                        UserHasActiveAppointment.Add(Convert.ToBoolean(parts[11]));
                        // Add the User Appointment Dates to list
                        UserAppointmentDates.Add(DateTime.Parse(parts[12].Trim())); // use Trim() to remove any extra spaces

                        // Update the last account number if this one is bigger
                        if (accNum > LastAccountNumber)
                            LastAccountNumber = accNum;
                    }
                }
                // Inform the user that accounts have been loaded successfully
                //Console.WriteLine("Accounts loaded successfully.");
                //Console.ReadLine();
            }
            catch// If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading file.");
                Console.ReadLine();
            }

        }
        public static void SaveAdminInformationToFile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(AdminInformationFilePath))
                {
                    // Loop through all accounts by index
                    for (int i = 0; i < AdminID.Count; i++)
                    {
                        // Create a line of data combining account info separated by commas
                        string dataLine = $"{AdminAccountNumber[i]},{AdminID[i]},{AdminName[i]}, {AccountAdminHashedPasswords[i]}";
                        // Write the data line into the file
                        writer.WriteLine(dataLine);
                    }
                }
                // Inform the user that accounts were saved successfully
                Console.WriteLine("Admin Accounts saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving file.");
            }
        }
        public static void LoadAdminInformationFromFile()
        {
            try  // Try to execute the code inside the block
            {
                // Check if the accounts file does not exist
                if (!File.Exists(AdminInformationFilePath))
                {
                    // Inform the user that no data was found
                    Console.WriteLine("No saved data found.");
                    // Exit the method early
                    return;
                }

                // Clear the list of Admin name
                AdminName.Clear();
                // Clear the list of Admin ID
                AdminID.Clear();
                // Clear the list of Admin account numbers
                AdminAccountNumber.Clear();
                // clear Admin account hashed passwords
                AccountAdminHashedPasswords.Clear();


                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(AdminInformationFilePath))
                {
                    string line; // Declare a variable to hold each line
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Split the line into parts separated by commas
                        string[] parts = line.Split(',');
                        // Convert the first part to an integer
                        int accNum = Convert.ToInt32(parts[0]);
                        // Add the account number to the list
                        AdminAccountNumber.Add(accNum);
                        // Add the account username to the list
                        AdminID.Add(parts[1]);
                        // Add the account user national ID to the list
                        AdminName.Add(parts[2]);
                        // Add the account hashed password to the list
                        AccountAdminHashedPasswords.Add(parts[3].Trim()); // add .Trim() to remove any extra spaces when loading the file
                        // Update the last account number if this one is bigger
                        if (accNum > LastAdminAccountNumber)
                            LastAdminAccountNumber = accNum;
                    }
                }
                // Inform the user that accounts have been loaded successfully
                //Console.WriteLine("Accounts loaded successfully.");
                //Console.ReadLine();
            }
            catch// If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading file.");
                Console.ReadLine();
            }

        }

        // ************************************ User Reviews Management ***************************************
        public static void SaveReviews()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(ReviewsFilePath))
                {
                    // Loop through all reviews
                    foreach (var review in UserReviewsStack)
                    {
                        // Write the review line into the file
                        writer.WriteLine(review);
                    }
                }
            }
            catch // // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving reviews.");
            }
        }
        public static void LoadReviews()
        {
            try // Try to execute the code inside the block
            {
                // Check if the accounts file does not exist
                if (!File.Exists(ReviewsFilePath)) return;
                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(ReviewsFilePath))
                {
                    // declare line variable to hold every line 
                    string line;
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // load the value of line in  UserReviewsStack
                        UserReviewsStack.Push(line);
                    }
                }
            }
            catch // If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading reviews.");
            }
        }    
        // ************************************ Accept Cancle reqest creation account ***************************************
        public static void SaveAccepteRequestsToFaile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(RequestsFilePath))
                {
                    // Loop through all reviews
                    foreach (var request in createAccountRequests)
                    {
                        // Write the review line into the file
                        writer.WriteLine(request);
                    }
                }
                // Inform the user that accounts were saved successfully
                Console.WriteLine("requests saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving request to file.");
            }
        }
        public static void LoadAcceptedRequests()
        {
            try // Try to execute the code inside the block
            {
                // Check if the accounts file does not exist
                if (!File.Exists(RequestsFilePath)) return;
                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(RequestsFilePath))
                {
                    // declare line variable to hold every line 
                    string line;
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // load the value of line in  UserReviewsStack
                        createAccountRequests.Enqueue(line);
                    }
                }
            }
            catch // If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading request.");
            }
        }
        public static void SaveCancleRequestsToFaile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(CancelcreateAccountRequestsFilePath))
                {
                    // Loop through all reviews
                    foreach (var InAcceptrequest in CancelcreateAccountRequests)
                    {
                        // Write the inAccept request line into the file
                        writer.WriteLine(InAcceptrequest);
                    }
                }
                // Inform the user that accounts were saved successfully
                Console.WriteLine("Cancled requests saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving Cancled requests to file.");
            }
        }
        public static void LoadCancleRequests()
        {
            try // Try to execute the code inside the block
            {
                // Check if the accounts file does not exist
                if (!File.Exists(CancelcreateAccountRequestsFilePath)) return;
                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(CancelcreateAccountRequestsFilePath))
                {
                    // declare line variable to hold every line 
                    string line;
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // load the value of line in  UserReviewsStack
                        CancelcreateAccountRequests.Enqueue(line);
                    }
                }
            }
            catch // If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading InAccept request.");
            }
        }

        // ************************************ save and load User Transactions ***************************************
        public static void SaveUserTransactionsToFile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(MonthlyStatementGeneratorFilePath))
                {
                    // Loop through all user transactions by index
                    for (int i = 0; i < UserTransactions.Count; i++)
                    {
                        // Write each transaction list as a single line in the file
                        writer.WriteLine(string.Join(";", UserTransactions[i]));
                    }


                }
                // Inform the user that transactions were saved successfully
              //Console.WriteLine("User transactions saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving user transactions to file.");
            }
        }
        public static void LoadUserTransactionsFromFile()
        {
            try // Try to execute the code inside the block
            {
                // Check if the transactions file does not exist
                if (!File.Exists(MonthlyStatementGeneratorFilePath)) return;
                // Clear the list of user transactions
                UserTransactions.Clear();
                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(MonthlyStatementGeneratorFilePath))
                {
                    string line; // Declare a variable to hold each line
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Split the line by semicolon and add it as a new transaction list
                        UserTransactions.Add(line.Split(';').ToList());
                    }
                }
                // Inform the user that transactions have been loaded successfully
                Console.WriteLine("User transactions loaded successfully.");
            }
            catch // If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading user transactions from file.");
            }
        }

        //****************************** save and load User Feedback ***************************************
        public static void SaveUserFeedbackToFile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(UserFeedbackFilePath))
                {
                    // Loop through all user feedback by index
                    for (int i = 0; i < UserFeedbacks.Count; i++)
                    {
                        // Write each feedback as a single line in the file
                        writer.WriteLine(UserFeedbacks[i]);
                    }
                }
                // Inform the user that feedback was saved successfully
                Console.WriteLine("User feedback saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving user feedback to file.");
            }
        }
        public static void LoadUserFeedbackFromFile()
        {
            try // Try to execute the code inside the block
            {
                // Check if the feedback file does not exist
                if (!File.Exists(UserFeedbackFilePath)) return;
                // Clear the list of user feedbacks
                UserFeedbacks.Clear();
                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(UserFeedbackFilePath))
                {
                    string line; // Declare a variable to hold each line
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Add each feedback line to the UserFeedbacks list
                        UserFeedbacks.Add(line);
                    }
                }
                // Inform the user that feedback has been loaded successfully
                Console.WriteLine("User feedback loaded successfully.");
            }
            catch // If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading user feedback from file.");
            }
        }

        // *************************************** Save and load Request Of appointment ***************************************
        public static void SaveAppointmentRequestsToFile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(AppointmentRequestsFilePath))
                {
                    // Loop through all appointment requests
                    foreach (var request in AppointmentRequests)
                    {
                        // Write the request line into the file
                        writer.WriteLine(request);
                        Console.WriteLine(request);
                        
                    }
                   
                }
                // Inform the user that appointment requests were saved successfully
                Console.WriteLine("Appointment requests saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving appointment requests to file.");
            }
        }
        public static void LoadAppointmentRequestsFromFile()
        {
            try // Try to execute the code inside the block
            {
                // Check if the appointment requests file does not exist
                if (!File.Exists(AppointmentRequestsFilePath)) return;
                // Clear the queue of appointment requests
                AppointmentRequests.Clear();
                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(AppointmentRequestsFilePath))
                {
                    string line; // Declare a variable to hold each line
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Enqueue each request line into the AppointmentRequests queue
                        AppointmentRequests.Enqueue(line);
                    }
                }
                // Inform the user that appointment requests have been loaded successfully
                Console.WriteLine("Appointment requests loaded successfully.");
            }
            catch // If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading appointment requests from file.");
            }
        }
        // ======================================== Backup All Datat =================================
        public static void BackupAllDataToFile()
        {
            string backupFileName = $"Backup_{DateTime.Now:yyyy-MM-dd_HHmm}.txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(backupFileName))
                {
                    writer.WriteLine("=== Account Data Backup ===");
                    for (int i = 0; i < AccountNumbers.Count; i++)
                    {
                        writer.WriteLine($"AccountNumber:{AccountNumbers[i]}, Name:{AccountUserNames[i]}, NationalID:{AccountUserNationalID[i]}, Balance:{UserBalances[i]}, Phone:{UserPhoneNumbers[i]}, Address:{UserAddresses[i]}, HasActiveLoan:{UserHasActiveLoan[i]}, LoanAmount:{UserLoanAmounts[i]}, InterestRate:{UserLoanInterestRates[i]}");
                    }

                    writer.WriteLine();
                    writer.WriteLine("=== Transaction Logs ===");
                    for (int i = 0; i < UserTransactions.Count; i++)
                    {
                        writer.WriteLine($"Transactions for {AccountUserNames[i]} (Account {AccountNumbers[i]}):");
                        foreach (var transaction in UserTransactions[i])
                        {
                            writer.WriteLine(transaction);
                        }
                        writer.WriteLine();
                    }
                }

                Console.WriteLine($"Backup completed successfully: {backupFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Backup failed: {ex.Message}");
            }
        }

        //************************************ check balance amount to decided if we can withdraw or not *********************************
        public static bool CheckBalanceAmount(double FinalAmount, int indexID)
        {
            // flag 
            bool GoWithProcess = false; 
            // check if balance has more than Minimum Balance 
            if ((UserBalances[IndexID] > MinimumBalance) && (FinalAmount <= (UserBalances[IndexID] - MinimumBalance)))
            {
                // put flag as true if user can go with process 
                GoWithProcess = true;
            }
            else
            {
                // put flag as flase if user can not go with process becoouse its balance has just 100.00$
                GoWithProcess = false;
            }

            return GoWithProcess;
        }


        // =========================================================================== User Enter ID and Password ===========================
        // ************************************* Methods for password *************************************
        public static int EnterUserPassword(int Index_ID)
        {
            int tries = 0;
            int Indexpassword = -1;
            bool UserExist = false;
            // Step 3: Validate Password
            bool passwordCorrect = false;

            do
            {
                Console.Write("Enter your password: ");
                string enteredPassword = ReadPassword(); // masked input
                string enteredHashed = HashPassword(enteredPassword);

                // Fetch the stored hashed password for this user
                bool PassExist = ExistPassword(enteredHashed);

                if (PassExist == true)
                {
                    passwordCorrect = true;

                }
                else
                {
                    Console.WriteLine("\nIncorrect password. Please try again.");
                    tries++;
                }

            } while (!passwordCorrect && tries < 3);

            if (tries == 3)
            {
                Console.WriteLine("You have exceeded the allowed attempts for password entry.");
                // Lock account after 3 failed attempts
                UserIsLocked[IndexID] = true;
                Console.WriteLine("Account locked due to 3 failed login attempts. Please contact admin to unlock.");
                Indexpassword = -1; // login fails
            }

            if (passwordCorrect == true)
            {
                Indexpassword = Index_ID;
            }
            return Indexpassword;
        }
        // Read password from console without echoing characters
        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
        // Hash the password using SHA256
        static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // vlidate exist User password in the list
        static bool ExistPassword(string HashPassword)
        {
            // Loop through the list of registered password
            for (int i = 0; i < AccountUserHashedPasswords.Count; i++)
            {
                // Check if the current password in the list matches the user's input
                if (AccountUserHashedPasswords[i] == HashPassword)
                {
                    return true; // User password exists in the list
                }
            }
            return false; // User password does not exist in the list
        }

        // vlidate exist Admin password in the list
        static bool AdminExistPassword(string HashPassword)
        {
            // Loop through the list of registered password
            for (int i = 0; i < AccountAdminHashedPasswords.Count; i++)
            {
                // Check if the current password in the list matches the user's input
                if (AccountAdminHashedPasswords[i] == HashPassword)
                {
                    return true; // User password exists in the list
                }
            }
            return false; // User password does not exist in the list
        }

        // ********************************************** Methods for User ID ****************************
        public static int EnterUserID()
        {
            int tries = 0;
            int IndexId = -1;
            bool UserExist = false;
            string ID = "";
            // Step 1: Verify National ID
            do
            {
                // Prompt user to enter their National ID
                Console.WriteLine("Enter User National ID: ");
                ID = Console.ReadLine(); // Read user input from console                           
                // valid user exist
                UserExist = CheckUserIDExist(ID);
                if (UserExist == false) // or if(!UserExist)
                {
                    Console.WriteLine("User with this ID does not exist. Please try again.");
                    tries++;
                }

                

            } while (UserExist == false && tries < 3);
            if (tries == 3)
            {
                Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid ID.");
                Console.ReadLine();
                return IndexId;
            }
            tries = 0;
            // Step 2: Find user ID index
            if (UserExist == true) // or if(UserExist)
            {
                //loop thriugh items in list
                for (int i = 0; i < AccountUserNationalID.Count; i++)
                {
                    //check if Input exist in the list 
                    if (AccountUserNationalID[i] == ID)
                    {
                        // Store the index of the user with the matching ID.
                        IndexId = i;
                        // Check if the account is locked
                        if (UserIsLocked[IndexId])
                        {
                            Console.WriteLine("This account is locked due to too many failed login attempts. Please contact the admin to unlock.");
                            return -1;
                        }

                    }
                }
            }
            return IndexId;
        }

        // ================================================================ Admin Enter ID and Password =====================

        public static int EnterAdminID()
        {
            int tries = 0;
            int AdminIndex = -1;
            string ID = "";
            do
            {
                // Prompt user to enter their National ID
                Console.WriteLine("Enter You National ID: ");
                ID = Console.ReadLine(); // Read user input from console
                AdminIndex = IDAdminExist(ID);
                if (AdminIndex == -1)
                {
                    tries++;
                }

            } while (AdminIndex == -1 && tries < 3);
            if (tries == 3)
            {
                Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid ID.");
                Console.ReadLine();
                return AdminIndex;
            }
            tries = 0;
            return AdminIndex;
        }
        public static int EnterAdminPassword(int Index_ID)
        {
            int tries = 0;
            int IndexId = -1;
            bool PassExist = false;


            do
            {
                Console.Write("Enter your password: ");
                string enteredPassword = ReadPassword().Trim(); // masked input
                string enteredHashed = HashPassword(enteredPassword);

                // Fetch the stored hashed password for this user
                PassExist = AdminExistPassword(enteredHashed);

                if (PassExist == true)
                {
                    IndexId = Index_ID;
                }
                else
                {
                    Console.WriteLine("\nIncorrect password. Please try again.");
                    tries++;
                }

            } while (PassExist ==false && tries < 3);

            if (tries == 3)
            {
                Console.WriteLine("You have exceeded the allowed attempts for password entry.");
                Console.ReadLine();
                IndexId = -1; // login fails
            }
            tries = 0;
            return IndexId;



        }



    }


}
