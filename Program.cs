using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniBankProject
{
    internal class Program

    {
        // Constants
        const double MinimumBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string ReviewsFilePath = "reviews.txt";
        // generate ID number for every account 
        static int LastAccountIDNumber = 0;
        static int IndexID = 0;
        // Global lists(parallel)
        static List<int> AccountIDNumbers = new List<int>();

        // Account data in Lists
        static List<string> AccountUserNames = new List<string>();
        static List<string> AccountUserNationalID = new List<string>();
        static List<double> UserBalances = new List<double>();

        //Requests in queue
        //static Queue<(string name, string nationalID)> createAccountRequests = new Queue<(string, string)>();
        static Queue<string> createAccountRequests = new Queue<string>(); // format: "Name|NationalID"

        //review in stack
        static Stack<string> UserReviews = new Stack<string>();
        // ======================================== Menu Functions =================================
        static void Main(string[] args)
        {
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
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. View Balance");
                Console.WriteLine("5. Submit Review/Complaint");
                Console.WriteLine("0. Return to Main Menu");
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
                        IndexID = LoginWithID();
                        if (IndexID != -1)
                        {
                            Console.WriteLine("Proceeding to deposit...");
                            Deposit(IndexID); // If user exists, proceed with deposit
                            Console.ReadLine(); // Wait for user input before continuing
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        break;
                    // case to Withdraw
                    case '3':
                        IndexID = LoginWithID();
                        if (IndexID != -1)
                        {
                            Console.WriteLine("Proceeding to deposit...");
                            withdraw(IndexID); // If user exists, proceed with withdraw
                            Console.ReadLine(); // Wait for user input before continuing
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        break;
                    // case to View Balance
                    case '4':
                        IndexID = LoginWithID();
                        if (IndexID != -1)
                        {
                            Console.WriteLine("Proceeding to deposit...");
                            CheckBalance(IndexID); // If user exists, proceed with chech balance
                            Console.ReadLine(); // Wait for user input before continuing
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        break;
                    // case to Submit Review/Complaint
                    case '5':
                        SubmitReview();
                        Console.ReadLine();
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
                // display All Admin Menu
                Console.Clear();
                Console.WriteLine("\n------ Admin Menu ------");
                Console.WriteLine("1. Process Next Account Request");
                Console.WriteLine("2. View Submitted Reviews");
                Console.WriteLine("3. View All Accounts");
                Console.WriteLine("4. View Pending Account Requests");
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

        // ===================== User Features Function ==========================
        // Request Account Creation fiunction
        public static void RequestAccountCreation()
        {
            // Error handling 
            try
            {
                // ask user to enter his name
                Console.WriteLine("Enter Your Name: ");
                string name = Console.ReadLine();
                // valid the name input 
                string ValidName = stringOnlyLetterValidation(name);
                // ask user to enter his national ID 
                Console.WriteLine("Enter your National ID: ");
                string ID = Console.ReadLine();
                // valid the ID input
                string ValidID = StringOnlyNumberValidation(ID);

                // save user name and id in the single string value 
                string request = ValidName + '|' + ValidID;
                // add into createAccountRequests queue 
                createAccountRequests.Enqueue(request);
                // display message submit successfully 
                Console.WriteLine("Request Account Creation successfully submit");
            }
            catch
            {
                // display message submit failed 
                Console.WriteLine("Request Account Creation failed submit");
            }

        }
        // Deposit Function 
        public static void Deposit(int IndexID)
        {
            // Initialize a boolean flag to control the deposit loop.
            bool IsDeposit = false;
            // Initialize a variable to store the final parsed deposit amount.
            double FinalDepositAmount = 0.0;
            // Initialize an index to find the user's position in the account list.

            // Start a try block to catch potential runtime exceptions.
            try
            {
                // Repeat until a valid deposit is made.
                while (IsDeposit == false)
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
                    }
                    // If input is valid, find the user index.
                    else
                    {
                        // convert string to double using TryParse
                        double.TryParse(DepositAmount, out FinalDepositAmount);

                        // Update the user's balance by adding the deposit amount.
                        UserBalances[IndexID] = UserBalances[IndexID]+FinalDepositAmount;
                        // Set the flag to true to exit the loop.
                        IsDeposit = true;
                        // Exit the method (if inside a method).
                        return;

                    }
                }
            }
            //Print any exception message that occurs during execution.
            catch (Exception e) { Console.WriteLine(e.Message); }

        }
        // Withdraw Function 
        public static void withdraw(int IndexID)
        {
            // Initialize a boolean flag to control the deposit loop.
            bool IsWithdraw = false;
            // Initialize a variable to store the final parsed deposit amount.
            double FinalwithdrawAmount = 0.0;
            // Initialize an index to find the user's position in the account list.
            
            // Start a try block to catch potential runtime exceptions.
            try
            {
                // Repeat until a valid deposit is made.
                while (IsWithdraw == false)
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
                    }
                    // If input is valid, find the user index.
                    else
                    {
                        
                        // convert string to double using TryParse
                        double.TryParse(WithdrawAmount, out FinalwithdrawAmount);
                        // check if user balamce is less than or equal MinimumBalance
                        if (UserBalances[IndexID] <= MinimumBalance)
                        {
                            Console.WriteLine("Can not withdraw from your balance, becouse your balance has less than or equal 100.00$");
                        }
                        else
                        {
                            // Update the user's balance by adding the deposit amount.
                            UserBalances[IndexID] = UserBalances[IndexID] - FinalwithdrawAmount;
                            // Set the flag to true to exit the loop.
                            IsWithdraw = true;
                            // Exit the method (if inside a method).
                        }
                        return;

                    }
                }
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
                    UserReviews.Push(review);
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
        // login user 
        public static bool UserLogin(string ID)
        {
            bool ValidUserLogin = true;
            // Start of try block to catch any unexpected runtime exceptions
            try
            {
                
                bool ValidID = NationalIDValidation(ID); // Validate the input ID using a validation method
                if (ValidID == true)  // Proceed only if the ID is valid
                {
                    bool userFound = false;
                    // Loop through the list of registered National IDs
                    for (int i = 0; i < AccountUserNationalID.Count; i++)
                    {
                        // Check if the current ID in the list matches the user's input
                        if (AccountUserNationalID[i] == ID)
                        {
                            userFound = true;  // If match found, set userFound = true
                            break;
                        }
                    }
                    if (userFound)
                    {
                        Console.WriteLine("Login successful!");
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

        // ===================== Admin Features Function ==========================
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
                Console.WriteLine($"{AccountIDNumbers[i]}\t{"|"}{AccountUserNames[i]}\t{"|"}{AccountUserNationalID[i]}\t{"|"}{UserBalances[i]}");

            }
            
        }
        // View Reviews Function 
        public static void ViewReviews()
        {
            //error handling using try-catch
            try
            {
                //iteration all users reviews in UserReviews stack 
                foreach (string Review in UserReviews)
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
                // Extract and store the national ID from the request
                string UserNationalID = SplitRrquest[1];
                // Increment the last account ID number for the new account
                int NewAccountIDNumber = LastAccountIDNumber + 1;
                // Set initial account balance to 0
                double balance = 0.0;
                // Add user name in the AccountUserNames list
                AccountUserNames.Add(UserName);
                // Add user national ID in the AccountUserNationalID list
                AccountUserNationalID.Add(UserNationalID);
                // Add user Account ID in the AccountIDNumbers list
                AccountIDNumbers.Add(NewAccountIDNumber);
                // Add user initial balance in the Balances list
                UserBalances.Add(balance);
                Console.WriteLine($"Account created for {UserName} with Account Number: {NewAccountIDNumber}");
                LastAccountIDNumber = NewAccountIDNumber;


            }
            catch
            {
                //display massage to the user if anyy error happened during running program 
                Console.WriteLine("Accept process fail, Try Agine!");
            }
            
        }

        // ************************************************* Validation **********************************************
        // string validation 
        public static string stringOnlyLetterValidation(string word)
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
                Console.WriteLine("Valid: only letters.");
                IsValid = true;
            }
            else
            {
                Console.WriteLine("Invalid: contains non-letter characters.");
                IsValid = false;
            }

            if (IsValid)
            {
                ValidWord = word;
            }
            else
            {
                Console.WriteLine("word unsaved");            
            }
            return ValidWord;
        }
        // validate numeric strting
        public static string StringOnlyNumberValidation(string word)
        {
            bool IsValid = true;
            string ValidWord = "";
            if (string.IsNullOrWhiteSpace(word))
            {
                Console.WriteLine("Input is just spaces or empty!");
                IsValid = false;

            }
            else
            {
                IsValid = true;
            }
            if (Regex.IsMatch(word, @"^\d+$"))
            {
                Console.WriteLine("Valid: only numbers.");
                IsValid = true;

            }
            else
            {
                Console.WriteLine("Invalid: contains non-numeric characters.");
                IsValid = false;
            }
            if (IsValid)
            {
                ValidWord = word;
            }
            else
            {
                Console.WriteLine("word unsaved! try agine");
            }
            return ValidWord;
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

        //NationalID validation formate
        public static bool NationalIDValidation(string NationalID)
        {
            // Check if the input is not null or empty
            if (!string.IsNullOrEmpty(NationalID))
            {
                // Check if input is exactly 8 digits and only contains numbers
                if (NationalID.Length == 8 && NationalID.All(char.IsDigit))
                {
                    //Console.WriteLine("Valid integer: " + NationalID);
                    return true;
                }
                else
                {
                    Console.WriteLine("National ID should be exactly 8 digits and numeric only.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Null integer value");
            }
           
            return false;
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
                            Console.WriteLine("Valid input: " + result);
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
        //


        // get index if id exist
        public static int LoginWithID()
        {
            int IndexId = -1;
            // Prompt user to enter their National ID
            Console.WriteLine("Enter You National ID: ");
            string ID = Console.ReadLine(); // Read user input from console
            bool UserExist = UserLogin(ID);
            if (UserExist==true)
            {
                //lopp thriugh items in list
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
            else
            {
                Console.WriteLine("Your ID number is not exist");
            }
            return IndexId;
        }

    }

 
}
