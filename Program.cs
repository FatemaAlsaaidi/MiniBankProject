using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiniBankProject
{
    internal class Program

    {
        // generate ID number for every account 
        static int AccountIDNumber;
        // Global lists(parallel)
        static List<int> AccountIDNumbers = new List<int>();
        static List<string> AccountNames = new List<string>();
        static List<double> Balances = new List<double>();

        //static Queue<(string name, string nationalID)> createAccountRequests = new Queue<(string, string)>();
        static Queue<string> createAccountRequests = new Queue<string>(); // format: "Name|NationalID"

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

                        break;
                    // case to View Submitted Reviews
                    case '2':

                        break;
                    // case to View All Accounts
                    case '3':

                        break;
                    // case to View Pending Account Requests
                    case '4':
                        
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
                string ValidID = StringWithNumberValidation(ID);

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

        }

        // ===================== Admin Features Function ==========================
        // View Pending Requests Function 
        public static void ViewPendingRequests()
        {

        }
        // View All Accounts Function 
        public static void ViewAllAccounts()
        {

        }
        // View Reviews Function 
        public static void ViewReviews()
        {

        }
        // Process Account Request Function 
        public static void ProcessAccountRequest()
        {
            foreach (string request in createAccountRequests)
            {
                string[] SplitRrquest = multiCharString.Split
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
        public static string StringWithNumberValidation(string word)
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
    }
}
