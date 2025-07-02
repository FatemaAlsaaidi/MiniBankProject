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

namespace MiniBankProject
{
    internal class Program

    {
        // Constants
        const double MinimumBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string ReviewsFilePath = "reviews.txt";
        const string RequestsFilePath = "requests.txt";
        const string InAcceptRequestsFilePath = "InRequests.txt";
        const string AdminInformationFilePath = "Admin.txt";
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


        // generate ID number for Admin account 
        static int LastAdminAccountNumber = 0;

        // Admin Login information 
        static List<int> AdminAccountNumber = new List<int>();
        static List<string> AdminName = new List<string>();
        static List<string> AdminID = new List<string>();
        static List<string> AccountAdminHashedPasswords = new List<string>();



        //Requests in queue
        //static Queue<(string name, string nationalID)> createAccountRequests = new Queue<(string, string)>();
        static Queue<string> createAccountRequests = new Queue<string>(); // format: "Name|NationalID"

        static Queue<string> InAcceptcreateAccountRequests = new Queue<string>(); // format: "Name|NationalID"

        //review in stack
        static Stack<string> UserReviewsStack = new Stack<string>();

        // Export All Account Info file path
        static string ExportFilePath = "ExportedAccounts.txt";

        // ======================================== Menu Functions =================================
        static void Main(string[] args)
        {
            Console.WriteLine("========== Wellcome To Bank System =======================");
            Console.ReadLine();
            LoadAccountsInformationFromFile();
            LoadReviews();
            LoadRequests();
            LoadAdminInformationFromFile();
            LoadInAcceptRequests();
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
                        SaveRequestsToFaile();
                        SaveAdminInformationToFile();
                        SaveInRequestsToFaile();
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
                        IndexID = UserLoginWithID();
                        if (IndexID != -1)
                        {
                            Console.WriteLine("Login successfully");
                            Console.ReadLine();
                            UserMenuOperations(IndexID);
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
                        IndexID = AdminLoginWithID();
                        Console.ReadLine(); // Wait for user input before continuing
                        if (IndexID !=-1)
                        {
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
            bool ValidName = true;
            string UserID = "";
            bool ValidID = true;
            bool IsSave = true;
            int tries = 0;
            string password = "";
            string hashedPassword = "";
            // Error handling 
            try
            {
                // Enter User Name Process
                do
                {
                    // ask user to enter his name
                    Console.WriteLine("Enter Your Name: ");
                    string name = Console.ReadLine();
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
                        string request = UserName + "|" + UserID + "|" + hashedPassword;
                        createAccountRequests.Enqueue(request);
                        Console.WriteLine("Request Account Creation successfully submitted.");
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
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select option: ");
                char userChoice = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (userChoice)
                {                    
                    // case to Deposit
                    case '1':
                        
                        Console.WriteLine("Proceeding to deposit...");
                        Deposit(IndexID); // If user exists, proceed with deposit
                        Console.ReadLine(); // Wait for user input before continuing
                      
                        break;
                    // case to Withdraw
                    case '2':
                        
                        Console.WriteLine("Proceeding to withdraw...");
                        withdraw(IndexID); // If user exists, proceed with withdraw
                        Console.ReadLine(); // Wait for user input before continuing
                       
                        break;
                    // case to View Balance
                    case '3':

                        Console.WriteLine("Proceeding to Check Balance...");
                        CheckBalance(IndexID); // If user exists, proceed with chech balance
                        Console.ReadLine(); // Wait for user input before continuing
                       
                        break;
                    // case to Submit Review/Complaint
                    case '4':
                        SubmitReview();
                        Console.ReadLine();
                        break;
                    // Transfer Money
                    case '5':
                        // Ask user to enter the National ID of the account to transfer money to
                        int UserIndexID2 = UserLoginWithID();
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
                    case '6':
                        UndoLastComplaint();
                        Console.ReadLine(); // Wait for user input before continuing
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
        // Deposit Function 
        public static void Deposit(int IndexID)
        {
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
                    //IndexID = LoginWithID();
                    Console.WriteLine("Enter the amount of money you want to deposit: ");
                    string DepositAmount = Console.ReadLine();
                    // Validate the entered amount using a custom method.
                    bool ValidDepositAmount = AmountValid(DepositAmount);
                    if (ValidDepositAmount == false)
                    {
                        // Display error if the input is not valid.
                        Console.WriteLine("Invalid input");
                        IsDeposit = false;
                        tries++;
                    }
                    // If input is valid, find the user index.
                    else
                    {
                        // convert string to double using TryParse
                        double.TryParse(DepositAmount, out FinalDepositAmount);

                        // Update the user's balance by adding the deposit amount.
                        UserBalances[IndexID] = UserBalances[IndexID] + FinalDepositAmount;
                        UserBalances[IndexID] = UserBalances[IndexID] + FinalDepositAmount;
                        PrintReceipt(transactionType: "Deposit", amount: FinalDepositAmount, balance: UserBalances[IndexID]);
                        // Set the flag to true to exit the loop.
                        IsDeposit = true;
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
                            PrintReceipt(transactionType: "Withdraw", amount: FinalwithdrawAmount, balance: UserBalances[IndexID]);

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
        public static int UserLoginWithID()
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
                if(UserExist == false) // or if(!UserExist)
                {
                    Console.WriteLine("User with this ID does not exist. Please try again.");
                    tries++;
                }

            } while (UserExist == false && tries <3);
            if (tries == 3)
            {
                Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid ID.");
                Console.ReadLine();
                return IndexId;
            }
            tries = 0;
            // Step 2: Find user index
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
                    }
                }
            }
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
                    Console.WriteLine("\nLogin successful.");
                }
                else
                {
                    Console.WriteLine("\nIncorrect password. Please try again.");
                    tries++;
                }

            } while (!passwordCorrect && tries < 3);

            if (!passwordCorrect)
            {
                Console.WriteLine("You have exceeded the allowed attempts for password entry.");
                IndexId = -1; // login fails
            }

            return IndexId;
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
            // Start a try block to catch potential runtime exceptions.
            int IndexId = -1;
            bool UserExist = false;
            try
            {
                // Repeat until a valid transfer is made.
                do
                {
                    // Ask user to enter the amount to transfer.
                    Console.WriteLine("Enter the amount of money you want to transfer: ");
                    TransferAmount = Console.ReadLine();
                    // Validate the entered amount using a custom method.
                    bool ValidTransferAmount = AmountValid(TransferAmount);
                    if (ValidTransferAmount == false)
                    {
                        // Display error if the input is not valid.
                        Console.WriteLine("Invalid input");
                        IsTransfer = false;
                        tries++;
                    }
                    else
                    {
                        // Convert string to double using TryParse
                        double.TryParse(TransferAmount, out FinalTransferAmount);
                        // Check if user balance is sufficient for the transfer.
                        bool checkBalance = CheckBalanceAmount(FinalTransferAmount, UserIndexID);
                        if (checkBalance == true)
                        {
                            // Update the sender's balance by subtracting the transfer amount.
                            UserBalances[UserIndexID] -= FinalTransferAmount;
                            // Update the receiver's balance by adding the transfer amount.
                            UserBalances[UserIndexID2] += FinalTransferAmount;
                            Console.WriteLine($"Successfully transferred {FinalTransferAmount} from Account {AccountNumbers[UserIndexID]} to Account {AccountNumbers[UserIndexID2]}");
                            Console.WriteLine($"Your Current Balance is {UserBalances[UserIndexID]}");
                            IsTransfer = true; // Set flag to true to exit loop.
                        }
                        else
                        {
                            Console.WriteLine($"Cannot transfer {FinalTransferAmount} from your balance, as it would result in a balance below 100.00.");
                        }
                        return; // Exit method after successful transfer.
                    }
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (IsTransfer == false && tries < 3);
            }
            catch (Exception e) { Console.WriteLine(e.Message); } // Print any exception message that occurs during execution.
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
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select option: ");
                char adminChoice = Console.ReadKey().KeyChar;
                Console.WriteLine();

                // use switch to select one of many code blocks to be executed
                switch (adminChoice)
                {
                    // case to Process Next Account Request
                    case '1':
                        ProcessAccountRequest();
                        Console.ReadLine();
                        break;
                    // case to View Submitted Reviews
                    case '2':
                        ViewReviews();
                        Console.ReadLine();
                        break;
                    // case to View All Accounts
                    case '3':
                        ViewAllAccounts();
                        Console.ReadLine();
                        break;
                    // case to View Pending Account Requests
                    case '4':
                        ViewPendingRequests();
                        Console.ReadLine();
                        break;
                    // Search user by enter user National ID
                    case '5':
                        int UserIndexID = UserLoginWithID();
                        SearchUserByNationalID(UserIndexID);
                        Console.ReadLine();
                        break;
                    // Show Total Bank Balance
                    case '6':
                        ShowTotalBankBalance();
                        Console.ReadLine();
                        break;
                    // Delete Account
                    case '7':
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
                    case '8':
                        ShowTop3RichestCustomers();
                        Console.ReadLine();
                        break;
                    // Export All Account Info to a New File (CSV or txt)
                    case '9':
                        ExportAccountsToFile(ExportFilePath);
                        Console.WriteLine($"All account information has been exported to {ExportFilePath}");
                        Console.ReadLine();
                        break;
                    // case to Return to Main Menu
                    case '0':
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
        public static int AdminLoginWithID()
        {
            int tries = 0;
            int IndexId = -1;
            bool AdminExist = false;
            string ID = "";
            do
            {
                // Prompt user to enter their National ID
                Console.WriteLine("Enter You National ID: ");
                ID = Console.ReadLine(); // Read user input from console
                AdminExist = AdminLogin(ID);
                if(AdminExist == false)
                {
                    tries++;
                }
                
            } while (AdminExist == false && tries <3);
            if (tries == 3)
            {
                Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid ID.");
            }
            tries = 0;
           
            // Step 3: Validate Password
            bool passwordCorrect = false;

            do
            {
                Console.Write("Enter your password: ");
                string enteredPassword = ReadPassword(); // masked input
                string enteredHashed = HashPassword(enteredPassword);

                // Fetch the stored hashed password for this user
                bool PassExist = AdminExistPassword(enteredHashed);
               
                if (PassExist == true)
                {
                    passwordCorrect = true;
                    Console.WriteLine("\nLogin successful.");
                }
                else
                {
                    Console.WriteLine("\nIncorrect password. Please try again.");
                    passwordCorrect = false;
                    tries++;
                }

            } while (passwordCorrect == false && tries < 3);

            if (passwordCorrect == false)
            {
                Console.WriteLine("You have exceeded the allowed attempts for password entry.");
                IndexId = -1; // login fails
            }

            if (AdminExist == true && passwordCorrect == true)
            {
                //loop thriugh items in list
                for (int i = 0; i < AdminID.Count; i++)
                {
                    //check if Input exist in the list 
                    if (AdminID[i] == ID)
                    {
                        // Store the index of the user with the matching ID.
                        IndexId = i;
                    }
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
            
            Console.WriteLine($"Account ID {"|"} User Name {"|"} National ID {"|"} Balance Amount");
            //iteration for loop all index values in lists
            for (int i = 0; i < AccountUserNationalID.Count; i++)
            {
                //display list values for every index
                Console.WriteLine($"{AccountNumbers[i]}\t{"|"}{AccountUserNames[i]}\t{"|"}{AccountUserNationalID[i]}\t{"|"}{UserBalances[i]}");

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

      


                    // add user type 
                    Console.WriteLine($"Account created for {UserName} with Account Number: {NewAccountIDNumber} and Password: {UserHashedPassword}");
                    // display message to the user that account created successfully
                    Console.WriteLine("Account Accepted successfully.");
                    LastAccountNumber = NewAccountIDNumber;
                }
                else
                {
                    Console.WriteLine("Account Dose not accept!");
                    string InAcceptRequest = request;
                    InAcceptcreateAccountRequests.Enqueue(InAcceptRequest);
                }

                //Console.WriteLine("Do you want to exist from this page? (y/n)");
                //char choice2 = Console.ReadKey().KeyChar;
                //if (choice == 'y' || choice == 'Y')
                //{
                //    string InAcceptRequest2 = InAcceptcreateAccountRequests.Dequeue();
                //    createAccountRequests.Enqueue(InAcceptRequest2);

                //}

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
            // Check if there are at least 3 users
            if (UserBalances.Count < 3)
            {
                Console.WriteLine("Not enough users to determine the top 3 richest customers.");
                return;
            }
            // Create a list of tuples containing user index and balance
            var userBalancesWithIndex = UserBalances.Select((balance, index) => (index, balance)).ToList();
            // Sort the list by balance in descending order
            var top3Richest = userBalancesWithIndex.OrderByDescending(x => x.balance).Take(3).ToList();
            // Display the top 3 richest customers
            Console.WriteLine("Top 3 Richest Customers:");
            foreach (var user in top3Richest)
            {
                Console.WriteLine($"Account Number: {AccountNumbers[user.index]}, Name: {AccountUserNames[user.index]}, Balance: {user.balance}");
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

        // ************************************************* Validation **********************************************
        // string validation 
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

        // validate string which letter with number
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
            for (int i = 0; i < AccountUserNationalID.Count; i++)
            {
                // Check if the current ID in the list matches the user's input
                if (AccountUserNationalID[i] == UserID)
                {
                    return true; // User ID exists in the list
                }
            }
            return false; // User ID does not exist in the list
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

        // valid user id
        /*public static bool UserLogin(string ID)
        {
            int IndexUserID = 0;
            bool ValidUserLogin = true;
            // Start of try block to catch any unexpected runtime exceptions
            try
            {

                bool ValidID = IDValidation(ID); // Validate the input ID using a validation method
                if (ValidID == true)  // Proceed only if the ID is valid
                {
                    bool userFound = false;
                    // Loop through the list of registered National IDs
                    for (int i = 0; i < AccountUserNationalID.Count; i++)
                    {
                        // Check if the current ID in the list matches the user's input
                        if (AccountUserNationalID[i] == ID)
                        {
                            IndexUserID = i;
                            userFound = true;  // If match found, set userFound = true
                            break;
                        }
                    }
                    if (userFound== false)
                    {
                        Console.WriteLine($"successfully enter this {AccountUserNationalID[IndexUserID]} account!");
                        ValidUserLogin = true;

                    }
                    else
                    {
                        // If loop completes with no match, show message
                        Console.WriteLine("User with this ID number not found. please create an account before do this process");
                        ValidUserLogin = false;  // User not found, so login fails

                    }
                }
                else
                {
                    Console.WriteLine("National ID is invalid! please try agine");
                    Console.WriteLine("National ID should be exactly 8 digits and numeric only.");
                    ValidUserLogin = false;
                }

            }
            catch (Exception e) // Catch any exceptions that occur
            {
                Console.WriteLine(e.Message); // Print the error message
                ValidUserLogin = false;
            }
            //Console.WriteLine($"UserLogin result: {ValidUserLogin}"); // Print the result of UserLogin for debugging
            return ValidUserLogin;

        }*/


        // valid Admin exist 
        public static bool AdminLogin(string ID)
        {
            bool ValidUserLogin = true;
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
                            userFound = true;  // If match found, set userFound = true
                            break;
                        }
                    }
                    if (userFound)
                    {
                        Console.WriteLine("Admin Login successful!");
                        ValidUserLogin = true;

                    }
                    else
                    {
                        // If loop completes with no match, show message
                        Console.WriteLine("User not found. please create an account before do this process");
                        ValidUserLogin = false;  // User not found, so login fails

                    }
                }
                else
                {
                    Console.WriteLine("National ID is invalid! please try agine");
                    Console.WriteLine("National ID should be exactly 8 digits and numeric only.");
                    ValidUserLogin = false;
                }

            }
            catch (Exception e) // Catch any exceptions that occur
            {
                Console.WriteLine(e.Message); // Print the error message
                ValidUserLogin = false;
            }
            //Console.WriteLine($"UserLogin result: {ValidUserLogin}"); // Print the result of UserLogin for debugging
            return ValidUserLogin;

        }

        
        // ************************* saved files and loaded them ****************************
        //1. save and load Account information

        // Define a static method to save account information to a file
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
                        string dataLine = $"{AccountNumbers[i]},{AccountUserNames[i]},{AccountUserNationalID[i]},{UserBalances[i]},{AccountUserHashedPasswords[i]}";
                        Console.WriteLine(dataLine);
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
        // Define a static method that loads account information from a file
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
                        AccountUserHashedPasswords.Add(parts[4]);
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

        // 2. save and load reviews 
        // Define a static method to save user reviews to a file 
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
        // Define a static method to load user reviews to a file 
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

        // 3. save and load reqest 
        //Define a static method to save user requests to a file
        public static void SaveRequestsToFaile()
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
        // Define a static method to load user requests to a file 
        public static void LoadRequests()
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

        //4. save and load admin information 
        // Define a static method to save account information to a file
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
        // Define a static method that loads admin account information from a file
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
                        AccountAdminHashedPasswords.Add(parts[3]);
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

        // 4. save and load inaccept reqest 
        //Define a static method to save user requests to a file
        public static void SaveInRequestsToFaile()
        {
            try // Try to execute the code inside the block
            {
                // Open the file for writing 
                using (StreamWriter writer = new StreamWriter(InAcceptRequestsFilePath))
                {
                    // Loop through all reviews
                    foreach (var InAcceptrequest in InAcceptcreateAccountRequests)
                    {
                        // Write the inAccept request line into the file
                        writer.WriteLine(InAcceptrequest);
                    }
                }
                // Inform the user that accounts were saved successfully
                Console.WriteLine("Inrequests saved successfully.");
            }
            catch // If any error occurs during saving
            {
                // Inform the user that there was an error saving the file
                Console.WriteLine("Error saving InAcceptrequest to file.");
            }
        }
        // Define a static method to load user requests to a file 
        public static void LoadInAcceptRequests()
        {
            try // Try to execute the code inside the block
            {
                // Check if the accounts file does not exist
                if (!File.Exists(InAcceptRequestsFilePath)) return;
                // Open the file for reading using StreamReader
                using (StreamReader reader = new StreamReader(InAcceptRequestsFilePath))
                {
                    // declare line variable to hold every line 
                    string line;
                    // Read each line until the end of the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // load the value of line in  UserReviewsStack
                        InAcceptcreateAccountRequests.Enqueue(line);
                    }
                }
            }
            catch // If any error happens
            {
                // Inform the user that there was an error loading the file
                Console.WriteLine("Error loading InAccept request.");
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


        // ************************************* Methods for password validation *************************************
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






    }


}
