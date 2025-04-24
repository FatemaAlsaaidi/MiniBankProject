using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiniBankProject
{
    internal class Program

    {
        // generate ID number for every account 
        static int LastAccountIDNumber = 0;
        // Global lists(parallel)
        static List<int> AccountIDNumbers = new List<int>();

        // Account data in Lists
        static List<string> AccountUserNames = new List<string>();
        static List<string> AccountUserNationalID = new List<string>();
        static List<double> Balances = new List<double>();

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

                        break;
                    // case to Withdraw
                    case '3':

                        break;
                    // case to View Balance
                    case '4':

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
        public static void Deposit()
        {
            Console.WriteLine("Enter the amount of money you want to deposit: ");
            string DepositAmount = Console.ReadLine();

        }
        // Withdraw Function 
        public static void withdraw()
        {

        }
        // Check Balance Function
        public static void CheckBalance()
        {

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
                Console.WriteLine($"{AccountIDNumbers[i]}\t{"|"}{AccountUserNames[i]}\t{"|"}{AccountUserNationalID[i]}\t{"|"}{Balances[i]}");

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
                Balances.Add(balance);
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
        // integer validation 

        public static int NationalIDValidation(string num)
        {
            int ValidNumber = 0;
            bool IsValid = true;
            if (int.TryParse(num, out int result) && num.Length == 8)
            {
                Console.WriteLine("Valid integer: " + result);
                IsValid = true;
            }
            else
            {
                Console.WriteLine("Invalid integer value.");
                IsValid = false;
            }
            if (IsValid)
            {
                ValidNumber = result;
            }
            else
            {
                Console.WriteLine("ID Number unsaved! try agine");
            }

            return ValidNumber;
        }

        // numeric validation 
        public static bool NumericValid(string num)
        {

        }
    }
}
