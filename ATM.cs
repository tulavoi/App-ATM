using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

namespace AppAtm
{
    internal class ATM
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            MenuLogin();
            Console.ReadKey();
        }


        static LinkedList<Card> cardList = new LinkedList<Card>();
        static LinkedList<Admin> adminList = new LinkedList<Admin>();
        static LinkedList<TransactionHistory> transactionList = new LinkedList<TransactionHistory>();
        static LinkedList<Account> accountList = new LinkedList<Account>();

        static string idUser;


        // ----------------------------------------------------Home-----------------------------------------------------
        /// <summary>
        /// Hiển thị menu login
        /// </summary>
        static void MenuLogin()
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            CenterWriteLine("╔═══════════════════════════════════╗");
            CenterWriteLine("║             MENU LOGIN            ║");
            CenterWriteLine("╠═══════════════════════════════════╣");
            CenterWriteLine("║ 1. Đăng nhập Admin                ║");
            CenterWriteLine("║ 2. Đăng nhập User                 ║");
            CenterWriteLine("║ 3. Thoát ứng dụng                 ║");
            CenterWriteLine("╚═══════════════════════════════════╝");
            Console.WriteLine();

            SelectFunctionInMenuLogin();
        }


        /// <summary>
        /// Cho phép chọn chức năng trong Menu Login
        /// </summary>
        static void SelectFunctionInMenuLogin()
        {
            CenterWrite("\t\t  HÃY CHỌN CHỨC NĂNG");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            ConsoleKeyInfo choice = Console.ReadKey(true);
            Console.ResetColor();
            Console.WriteLine();

            // Xử lý chọn chức năng dựa trên phím bấm của người dùng
            switch (choice.Key)
            {
                // Bấm số 1 để đăng nhập dành cho Admin 
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    LoginAdmin();
                    break;

                // Bấm số 2 để đăng nhập dành cho User
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    LoginUser();
                    break;

                // Bấm số 3 để thoát ứng dụng
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    return;

                // Xử lý trường hợp chọn chức năng không hợp lệ
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(MenuLogin, MenuLogin, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Hiển thị thông báo lựa chọn không hợp lệ, cho phép lựa chọn nhập lại hoặc quay lại
        /// </summary>
        static void ChooseResetOrBackOrHome(Action f5Action, Action backAction, Action homeAction)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.F5:
                    Console.Clear();
                    f5Action();
                    break;

                case ConsoleKey.B:
                    backAction();
                    break;

                case ConsoleKey.H:
                    homeAction();
                    break;

                default:
                    break;
            }
        }


        // ----------------------------------------------------Admin-----------------------------------------------------
        /// <summary>
        /// Hiển thị giao diện đăng nhập dành cho admin.
        /// Yêu cầu người dùng nhập username và password, sau đó kiểm tra và xác thực.
        /// </summary>
        static void LoginAdmin()
        {
            Console.Clear();
            ReturnHomeOrContinue();

            // Giao diện trang đăng nhập admin
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            CenterWriteLine("╔═══════════════════════════════════════╗");
            CenterWriteLine("║              Login Admin              ║");
            CenterWriteLine("╠═══════════════════════════════════════╣");
            Console.ForegroundColor = ConsoleColor.White;
            CenterWrite("    Username: ");
            string usernameAdmin = Console.ReadLine();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            CenterWriteLine("╠═══════════════════════════════════════╣");
            Console.ForegroundColor = ConsoleColor.White;
            CenterWrite("    Password: ");
            string passAdmin = EncryptPassword();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();
            CenterWriteLine("╚═══════════════════════════════════════╝");
            Console.ResetColor();

            CheckUsernameAndPasswordOfAdmin(usernameAdmin, passAdmin);
        }


        /// <summary>
        /// Kiểm tra thông tin đăng nhập của tài khoản Admin.
        /// Đọc danh sách các tài khoản Admin từ tệp "Admin.txt", so sánh thông tin đăng nhập với danh sách.
        /// Nếu thông tin đúng, chuyển đến trang quản lý Admin (Menu Admin), ngược lại hiển thị thông báo lỗi đăng nhập.
        /// </summary>
        /// <param name="usernameAdmin">Username do người dùng nhập</param>
        /// <param name="passAdmin">Password do người dùng nhập</param>
        static void CheckUsernameAndPasswordOfAdmin(string usernameAdmin, string passAdmin)
        {
            adminList = new LinkedList<Admin>();
            try
            {
                // Đọc dữ liệu trong Admin.txt, thêm username và password vào adminList
                string[] lines = File.ReadAllLines("E:\\AppATM\\Admin.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split('#');
                    if (parts.Length == 2)
                    {
                        string username = parts[0];
                        string pass = parts[1];
                        adminList.AddLast(new Admin(username, pass));
                    }
                }

                // Kiểm tra username và password, nếu đúng chuyển đến MenuAdmin
                //bool isMatch = false;
                foreach (var admin in adminList)
                {
                    if (usernameAdmin == admin.Username && passAdmin == admin.Password)
                    {
                        MenuAdmin();
                        //isMatch = true;
                        break;
                    }
                    else if (usernameAdmin != admin.Username)
                    {
                        ShowAlertWrongUsername();
                        ChooseResetOrBackOrHome(LoginAdmin, MenuLogin, MenuLogin);
                    }
                    else if (usernameAdmin == admin.Username && passAdmin != admin.Password)
                    {
                        ShowAlertWrongPassword();
                        ChooseResetOrBackOrHome(LoginAdmin, MenuLogin, MenuLogin);
                    }
                }

                // Hiển thị thông báo nếu sai thông tin đăng nhập
                //if (!isMatch)
                //{
                //    ShowAlertWrongUsername();
                //    ChooseResetOrBackOrHome(LoginAdmin, MenuLogin, MenuLogin);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        /// <summary>
        /// Hiển thị thông báo sai mật khẩu
        /// </summary>
        static void ShowAlertWrongPassword()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            CenterWriteLine("╔═══════════════════════════════════╗");
            CenterWriteLine("║  Sai mật khẩu!                    ║");
            CenterWriteLine("║  (F5: Reset - B: Back - H: Home)  ║");
            CenterWriteLine("╚═══════════════════════════════════╝");
        }


        /// <summary>
        /// Hiển thị giao diện menu dành cho admin.
        /// Admin có thể chọn các chức năng trong menu để quản lý tài khoản.
        /// </summary>
        static void MenuAdmin()
        {
            Console.Clear();

            // Giao diện trang menu của admin
            Console.OutputEncoding = Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();
            CenterWriteLine("╔═════════════════════════════════╗");
            CenterWriteLine("║            MENU ADMIN           ║");
            CenterWriteLine("╠═════════════════════════════════╣");
            CenterWriteLine("║ 1. Xem Danh Sách Tài Khoản      ║");
            CenterWriteLine("║ 2. Thêm Tài Khoản               ║");
            CenterWriteLine("║ 3. Xóa Tài Khoản                ║");
            CenterWriteLine("║ 4. Mở Khóa Tài Khoản            ║");
            CenterWriteLine("║ B. Quay lại                     ║");
            CenterWriteLine("║ H. Home (Menu Login)            ║");
            CenterWriteLine("╚═════════════════════════════════╝");
            Console.WriteLine();

            SelectFunctionInMenuAdmin();
        }


        /// <summary>
        /// Hho phép chọn chức năng trong Menu Admin.
        /// </summary>
        static void SelectFunctionInMenuAdmin()
        {
            // Chọn chức năng trong menu login
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            CenterWrite("\t\t  HÃY CHỌN CHỨC NĂNG");
            ConsoleKeyInfo choose = Console.ReadKey(true);
            Console.ResetColor();

            // Xử lý chọn chức năng dựa trên phím bấm của người dùng
            switch (choose.Key)
            {
                // Bấm số 1 để thực hiện chức năng xem tài khoản
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.Clear();
                    DisplayViewAccountList();
                    break;

                // Bấm số 2 để thực hiện chức năng thêm tài khoản
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.Clear();
                    DisplayAddAccount();
                    break;

                // Bấm số 3 để thực hiện chức năng xóa tài khoản
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.Clear();
                    DisplayDeleteAccount();
                    break;

                // Bấm số 3 để thực hiện chức năng xóa tài khoản
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    Console.Clear();
                    DisplayUnlockAccount();
                    break;

                // Bấm B sẽ quay về trang Login Admin
                case ConsoleKey.B:
                    LoginAdmin();
                    break;

                // Bấm H sẽ quay về trang Login Admin
                case ConsoleKey.H:
                    MenuLogin();
                    break;

                // Xử lý trường hợp chọn chức năng không hợp lệ
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(MenuAdmin, LoginAdmin, MenuLogin);
                    break;
            }
            Console.ResetColor();
        }


        /// <summary>
        /// Chức năng 1. Hiển thị giao diện của chức năng xem tài khoản.
        /// Admin có thể chọn xem, quay lại hoặc trở về menu login.
        /// </summary>
        static void DisplayViewAccountList()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔═══════════════════════════════╗");
            CenterWriteLine("║    XEM DANH SÁCH TÀI KHOẢN    ║");
            CenterWriteLine("╠═══════════════════════════════╣");
            CenterWriteLine("║1. Tiến hành xem               ║");
            CenterWriteLine("║B. Quay lại                    ║");
            CenterWriteLine("║H. Home (Menu Login)           ║");
            CenterWriteLine("╚═══════════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    ViewAccountList(ShowListCard);
                    GoBackOrHome(MenuAdmin, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuAdmin();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayViewAccountList, MenuAdmin, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Xem danh sách tài khoản của chức năng 1.
        /// </summary>
        static void ViewAccountList(Action showAction)
        {
            cardList = ReadFileCards();
            showAction();
        }


        /// <summary>
        /// Hiển thị toàn bộ thẻ có trong dữ liệu
        /// </summary>
        static void ShowListCard()
        {
            Console.ForegroundColor = ConsoleColor.White;
            CenterWriteLine("╔═════════════════════════╗");
            CenterWriteLine("║   Danh sách tài khoản   ║");
            CenterWriteLine("╠════════════════╤════════╣");
            CenterWriteLine("║       ID       │  PIN   ║");
            CenterWriteLine("╟────────────────┼────────╢");
            foreach (var card in cardList)
                CenterWriteLine($"║ {card.Id,-14} │ {card.Pin,-6} ║");

            CenterWriteLine("╚════════════════╧════════╝");
            Console.ResetColor();
        }


        /// <summary>
        /// Chức năng 2. Hiển thị giao diện của chức năng Thêm Tài Khoản.
        /// </summary>
        static void DisplayAddAccount()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔═════════════════════════╗");
            CenterWriteLine("║      THÊM TÀI KHOẢN     ║");
            CenterWriteLine("╠═════════════════════════╣");
            CenterWriteLine("║1. Tiến hành thêm        ║");
            CenterWriteLine("║B. Quay lại              ║");
            CenterWriteLine("║H. Home (Menu Login)     ║");
            CenterWriteLine("╚═════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    AddAccount();
                    GoBackOrHome(MenuAdmin, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuAdmin();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayAddAccount, MenuAdmin, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Thêm tài khoản của chức năng 2.
        /// </summary>
        static void AddAccount()
        {
            ViewAccountList(ShowListCard);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n- Nhập ID (14 số): ");
            string newID = Console.ReadLine();

            CheckIDToAdd(newID);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n- Nhập họ tên: ");
            string fullName = Console.ReadLine();
            Console.Write("- Nhập loại tiền tệ: ");
            string typeCurrency = Console.ReadLine();
            Console.Write("- Nhập số dư: ");
            long balance = long.Parse(Console.ReadLine());

            Account newAccount = new Account(newID, fullName, typeCurrency, balance);
            accountList.AddLast(newAccount);
            Card newCard = new Card(newID, "123456", 0);
            cardList.AddLast(newCard);

            CreateAccountFile(newAccount);
            CreateTransactionFile(newAccount);
            UpdateFileCards();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            CenterWriteLine("╔ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╗");
            CenterWriteLine("    THÊM THÀNH CÔNG    ");
            CenterWriteLine("╚ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╝");
            CenterWriteLine("H To Home - B To Menu Admin");
        }


        /// <summary>
        /// Kiểm tra ID cần thêm.
        /// </summary>
        /// <param name="newID">ID cần thêm</param>
        static void CheckIDToAdd(string newID)
        {
            bool isIDExist = cardList.Any(card => card.Id == newID);
            while (isIDExist || !IsValidID(newID))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                if (!IsValidID(newID))
                {
                    Console.Write("- ID không hợp lệ, nhập lại: ");
                }
                else if (isIDExist)
                {
                    Console.Write("- ID đã tồn tại, nhập lại: ");
                }

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                newID = Console.ReadLine();
                isIDExist = cardList.Any(card => card.Id == newID);
                IsValidID(newID);
            }
        }


        /// <summary>
        /// Chức năng 3. Hiển thị tiêu đề của chức năng Xóa Tài Khoản.
        /// </summary>
        static void DisplayDeleteAccount()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔═════════════════════════╗");
            CenterWriteLine("║      XÓA TÀI KHOẢN      ║");
            CenterWriteLine("╠═════════════════════════╣");
            CenterWriteLine("║1. Tiến hành xóa         ║");
            CenterWriteLine("║B. Quay lại              ║");
            CenterWriteLine("║H. Home (Menu Login)     ║");
            CenterWriteLine("╚═════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    DeleteAccount();
                    GoBackOrHome(MenuAdmin, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuAdmin();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayDeleteAccount, MenuAdmin, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Xóa tài khoản của chức năng 3.
        /// </summary>
        static void DeleteAccount()
        {
            ViewAccountList(ShowListCard);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n- Nhập ID (14 số): ");
            string idToDelete = Console.ReadLine();

            CheckIDIsExist(idToDelete, DisplayDeleteAccount, MenuAdmin);

            // Tìm thẻ cần xóa trong danh sách dựa vào ID đã nhập
            Card deletedCard = cardList.First(card => card.Id == idToDelete);
            // Xóa thẻ đã tìm thấy ra khỏi danh sách cardList
            cardList.Remove(deletedCard);

            DeleteAccountFile(idToDelete);
        }


        /// <summary>
        /// Xóa file có tên tương ứng với ID đã nhập.
        /// Cập nhật lại file Cards.txt.
        /// </summary>
        /// <param name="idToDelete">Tên file cần xóa</param>
        static void DeleteAccountFile(string idToDelete)
        {
            string filePath = $"E:\\AppATM\\{idToDelete}.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                UpdateFileCards();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                CenterWriteLine("╔ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╗");
                CenterWriteLine("    XÓA THÀNH CÔNG     ");
                CenterWriteLine("╚ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╝");
                CenterWriteLine("H To Home - B To Menu Admin");
            }
        }


        /// <summary>
        /// Kiểm tra ID có tồn tại trong dữ liệu hay không.
        /// </summary>
        /// <param name="id">ID cần kiểm tra</param>
        static void CheckIDIsExist(string id, Action f5Action, Action backAction)
        {
            bool isIDExist = cardList.Any(card => card.Id == id);
            Console.WriteLine();
            if (!isIDExist || !IsValidID(id))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("- Tài khoản tồn tại hoặc không hợp lệ!");
                Console.Write("\nF5 - nhập lại, B - trở về, H - Menu Login");
                ChooseResetOrBackOrHome(f5Action, backAction, MenuLogin);
            }
            
        }


        /// <summary>
        /// Chức năng 4. Hiển thị tiêu đề của chức năng Mở Khóa Tài Khoản.
        /// </summary>
        static void DisplayUnlockAccount()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔═════════════════════════╗");
            CenterWriteLine("║     MỞ KHÓA TÀI KHOẢN   ║");
            CenterWriteLine("╠═════════════════════════╣");
            CenterWriteLine("║1. Tiến hành mở khóa     ║");
            CenterWriteLine("║B. Quay lại              ║");
            CenterWriteLine("║H. Home (Menu Login)     ║");
            CenterWriteLine("╚═════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    UnlockAccount();
                    GoBackOrHome(MenuAdmin, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuAdmin();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayDeleteAccount, MenuAdmin, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Xử lý việc mở khóa tài khoản của chức năng 4
        /// </summary>
        static void UnlockAccount()
        {
            ViewAccountList(ShowListLockedCard);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n- Nhập ID cần mở khóa: ");
            string idToUnlock = Console.ReadLine();
            CheckIDIsExist(idToUnlock, DisplayUnlockAccount, MenuAdmin);
            cardList = ReadFileCards();
            foreach (var card in cardList)
            {
                if (card.Id == idToUnlock && card.LoginAttemptsCount == 3)
                {
                    card.LoginAttemptsCount = 0;
                    UpdateFileCards();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    CenterWriteLine("╔ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╗");
                    CenterWriteLine("   MỞ KHÓA THÀNH CÔNG  ");
                    CenterWriteLine("╚ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╝");
                    CenterWriteLine("H To Home - B To Menu Admin");
                }
            }
        }


        /// <summary>
        /// Hiển thị danh sách thẻ bị khóa
        /// </summary>
        static void ShowListLockedCard()
        {
            Console.ForegroundColor = ConsoleColor.White;
            CenterWriteLine("╔═════════════════════════════╗");
            CenterWriteLine("║ Danh sách tài khoản bị khóa ║");
            CenterWriteLine("╠════════════════╤════════════╣");
            CenterWriteLine("║       ID       │    PIN     ║");
            CenterWriteLine("╟────────────────┼────────────╢");
            foreach (var card in cardList)
            {
                if (card.LoginAttemptsCount == 3)
                    CenterWriteLine($"║ {card.Id,-14} │ {card.Pin,-10} ║");
            }

            CenterWriteLine("╚════════════════╧════════════╝");
            Console.ResetColor();
        }


        // ----------------------------------------------------User-----------------------------------------------------
        /// <summary>
        /// Hiển thị giao diện đăng nhập dành cho user.
        /// Yêu cầu người dùng nhập id và mã pin, sau đó kiểm tra và xác thực.
        /// </summary>
        static void LoginUser()
        {
            Console.Clear();
            ReturnHomeOrContinue();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            CenterWriteLine("╔══════════════════════════════════════╗");
            CenterWriteLine("║              Login User              ║");
            CenterWriteLine("╠══════════════════════════════════════╣");
            Console.ForegroundColor = ConsoleColor.White;
            CenterWrite("    ID: ");
            idUser = Console.ReadLine();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            CenterWriteLine("╠══════════════════════════════════════╣");
            Console.ForegroundColor = ConsoleColor.White;
            CenterWrite("    PIN: ");
            string pinUser = EncryptPassword();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();
            CenterWriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();

            CheckIDAndPinOfUser(idUser, pinUser);
        }


        /// <summary>
        /// Kiểm tra id và mã pin được nhập có khớp với dữ liệu hay không
        /// </summary>
        /// <param name="id">ID do người dùng nhập</param>
        /// <param name="pin">Mã PIN do người dùng nhập</param>
        static void CheckIDAndPinOfUser(string idUser, string pinUser)
        {
            cardList = ReadFileCards();
            bool isMatch = false;
            foreach (var card in cardList)
            {
                if (card.LoginAttemptsCount == 3 && idUser == card.Id)
                    ShowAlertAccountLocked();
                else if (idUser == card.Id)
                {
                    if (card.Pin == pinUser)
                    {
                        card.LoginAttemptsCount = 0;
                        UpdateFileCards();
                        MenuUser();
                        isMatch = true;
                        break;
                    }
                    else
                    {
                        card.LoginAttemptsCount++;
                        UpdateFileCards();
                        ShowAlertWrongPassword();
                        ChooseResetOrBackOrHome(LoginUser, MenuLogin, MenuLogin);
                    }
                }
            }
            if (!isMatch)
            {
                ShowAlertWrongUsername();
                ChooseResetOrBackOrHome(LoginUser, MenuLogin, MenuLogin);
            }
        }

        
        /// <summary>
        /// Đọc file Cards
        /// </summary>
        /// <returns></returns>
        static LinkedList<Card> ReadFileCards()
        {
            var tempList = new LinkedList<Card>();
            try
            {
                string[] lines = File.ReadAllLines("E:\\AppATM\\Cards.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split('#');
                    if (parts.Length == 3)
                    {
                        string id = parts[0];
                        string pin = parts[1];
                        int loginAttemptsCount = Convert.ToInt32(parts[2]);
                        tempList.AddLast(new Card(id, pin, loginAttemptsCount));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            return tempList;
        }


        /// <summary>
        /// Hiển thị thông báo tài khoản đã bị khóa
        /// </summary>
        static void ShowAlertAccountLocked()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            CenterWriteLine("╔════════════════════════════════════╗");
            CenterWriteLine("║  Tài khoản đã bị khóa!             ║");
            CenterWriteLine("║  (F5: Reset - B: Back - H: Home)   ║");
            CenterWriteLine("╚════════════════════════════════════╝");
            ChooseResetOrBackOrHome(LoginUser, MenuLogin, MenuLogin);
        }


        /// <summary>
        /// Hiển thị giao diện menu dành cho user.
        /// User có thể chọn các chức năng trong menu.        
        /// </summary>
        static void MenuUser()
        {
            Console.Clear();
            Console.OutputEncoding = Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();
            CenterWriteLine("╔════════════════════════════════╗");
            CenterWriteLine("║            MENU USER           ║");
            CenterWriteLine("╠════════════════════════════════╣");
            CenterWriteLine("║ 1. Xem Thông Tin Tài Khoản     ║");
            CenterWriteLine("║ 2. Rút Tiền                    ║");
            CenterWriteLine("║ 3. Chuyển Tiền                 ║");
            CenterWriteLine("║ 4. Xem Nội Dung Giao Dịch      ║");
            CenterWriteLine("║ 5. Đổi Mật Khẩu                ║");
            CenterWriteLine("║ B. Quay lại                    ║");
            CenterWriteLine("║ H. Home (Menu Login)           ║");
            CenterWriteLine("╚════════════════════════════════╝");
            Console.WriteLine();

            SelectFunctionInMenuUser();
        }


        /// <summary>
        /// Cho phép chọn chức năng trong Menu User
        /// </summary>
        static void SelectFunctionInMenuUser()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            CenterWrite("\t\t   HÃY CHỌN CHỨC NĂNG");
            ConsoleKeyInfo choose = Console.ReadKey(true);
            Console.ResetColor();

            switch (choose.Key)
            {
                // Bấm số 1 để thực hiện chức năng Xem Thông Tin Tài Khoản
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.Clear();
                    DisplayViewAccount();
                    break;

                // Bấm số 2 để thực hiện chức năng Rút Tiền
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.Clear();
                    DisplayWithdrawMoney();
                    break;

                // Bấm số 3 để thực hiện chức năng Chuyển Tiền
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.Clear();
                    DisplayTransferMoney();
                    break;

                // Bấm số 4 để thực hiện chức năng Chuyển Tiền
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    Console.Clear();
                    DisplayViewTransaction();
                    break;

                // Bấm số 5 để thực hiện chức năng Đổi Mật Khẩu
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    Console.Clear();
                    DisplayChangePassword();
                    break;

                // Bấm B sẽ quay về trang Login Admin
                case ConsoleKey.B:
                    LoginUser();
                    break;

                // Bấm H sẽ quay về trang Login Admin
                case ConsoleKey.H:
                    MenuLogin();
                    break;

                // Xử lý trường hợp chọn chức năng không hợp lệ
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(MenuUser, LoginUser, MenuLogin);
                    break;
            }
            Console.ResetColor();
        }


        /// <summary>
        /// Chức năng 1. Hiển thị giao diện của chức năng Xem Thông Tin Tài Khoản
        /// User có thể chọn xem, quay lại hoặc trở về menu login.
        /// </summary>
        static void DisplayViewAccount()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔═══════════════════════════╗");
            CenterWriteLine("║  XEM THÔNG TIN TÀI KHOẢN  ║");
            CenterWriteLine("╠═══════════════════════════╣");
            CenterWriteLine("║1. Tiến hành xem           ║");
            CenterWriteLine("║B. Quay lại                ║");
            CenterWriteLine("║H. Home (Menu Login)       ║");
            CenterWriteLine("╚═══════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    ViewAccount();
                    GoBackOrHome(MenuUser, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuUser();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayViewAccount, MenuUser, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Xem thông tin tài khoản của chức năng 1
        /// </summary>
        static void ViewAccount()
        {
            Account account = ReadFileIDUser(idUser);
            ShowAccount(account);
        }


        /// <summary>
        /// Đọc file có tên tương ứng với ID người dùng nhập
        /// </summary>
        /// <returns></returns>
        static Account ReadFileIDUser(string idUser)
        {
            string filePath = $"E:\\AppATM\\{idUser}.txt";
            try
            {
                // Đọc dữ liệu trong file có tên tương ứng với id
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('#');
                    if (parts.Length == 4)
                    {
                        string id = parts[0];
                        string name = parts[1];
                        string typeCurrency = parts[2];
                        long balance = long.Parse(parts[3]);
                        Account account = new Account(id, name, typeCurrency, balance);
                        return account;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            return null;
        }


        /// <summary>
        /// Hiển thị thông tin tài khoản
        /// </summary>
        static void ShowAccount(Account account)
        {
            Console.ForegroundColor = ConsoleColor.White;
            CenterWriteLine("╔══════════════════════════════════════════════════════════════╗");
            CenterWriteLine("║                     Thông tin tài khoản                      ║");
            CenterWriteLine("╠════════════════╤═════════════════════════╤═══════════════════╣");
            CenterWriteLine("║       ID       │         Họ tên          │       Số dư       ║");
            CenterWriteLine("╟────────────────┼─────────────────────────┼───────────────────╢");
            CenterWriteLine($"║ {account.Id,-14} │ {account.Name,-23} │ {account.Balance + account.TypeCurrency,-17} ║");
            CenterWriteLine("╚════════════════╧═════════════════════════╧═══════════════════╝");
            Console.ResetColor();
        }


        /// <summary>
        /// Chức năng 1. Hiển thị giao diện của chức năng Rút Tiền
        /// User có thể chọn rút tiền, quay lại hoặc trở về menu login.        
        /// </summary>
        static void DisplayWithdrawMoney()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔══════════════════════════╗");
            CenterWriteLine("║         RÚT TIỀN         ║");
            CenterWriteLine("╠══════════════════════════╣");
            CenterWriteLine("║1. Tiến hành rút tiền     ║");
            CenterWriteLine("║B. Quay lại               ║");
            CenterWriteLine("║H. Home (Menu Login)      ║");
            CenterWriteLine("╚══════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    WithdrawMoney();
                    GoBackOrHome(MenuUser, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuUser();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayWithdrawMoney, MenuUser, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Xử lý việc rút tiền của chức năng 2
        /// </summary>
        static void WithdrawMoney()
        {
            ViewAccount();
            Account account = ReadFileIDUser(idUser);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n- Nhập số tiền cần rút: ");
            long amountWithdraw = long.Parse(Console.ReadLine());

            if (CheckTransactionAmount(amountWithdraw, account.Balance, DisplayWithdrawMoney))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                CenterWriteLine("╔ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╗");
                CenterWriteLine("  RÚT TIỀN THÀNH CÔNG  ");
                CenterWriteLine("╚ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╝");
                CenterWriteLine("H To Home - B To Menu Admin");
            }

            UpdateFileAccount(account, -amountWithdraw);

            TransactionHistory withdrawHistory = new TransactionHistory(idUser, "Rút tiền", amountWithdraw, DateTime.Now);
            WriteTransactionHistory(withdrawHistory, idUser);
        }


        /// <summary>
        /// Ghi thông tin giao dịch vào file
        /// </summary>
        /// <param name="idUser">ID thực hiện giao dịch</param>
        /// <param name="amountWithdraw">Số tiền giao dịch</param>
        static void WriteTransactionHistory(TransactionHistory transaction, string transactionID)
        {
            using (StreamWriter streamWriter = File.AppendText($"E:\\AppATM\\LichSu{transactionID}.txt"))
                streamWriter.WriteLine($"{transaction.TransactionType}#{transaction.Id}#{transaction.AmountMoney}#{transaction.TransactionTime}");
        }


        /// <summary>
        /// Kiểm tra số tiền cần rút hoặc chuyển.
        /// Giao dịch thành công khi số tiền cần rút/chuyển lớn hơn 20000, nhỏ hơn số dư - 50000.
        /// </summary>
        /// <param name="amountWithdraw">Số tiền cần thực hiện giao dịch</param>
        /// <param name="balance">Số dư</param>
        /// <returns></returns>
        static bool CheckTransactionAmount(long amount, long balance, Action f5Action)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (amount > balance - 20000)
            {
                Console.WriteLine("- Số dư không đủ để thực hiện giao dịch!");
                Console.Write("\nF5 - nhập lại, B - trở về, H - Menu Login");
                ChooseResetOrBackOrHome(f5Action, MenuUser, MenuLogin);
                return false;
            }
            else if (amount < 20000)
            {
                Console.WriteLine("- Số tiền cần rút/chuyển phải lớn hơn 20000VND!");
                Console.Write("\nF5 - nhập lại, B - trở về, H - Menu Login");
                ChooseResetOrBackOrHome(f5Action, MenuUser, MenuLogin);
                return false;
            }
            Console.ResetColor();
            return true;
        }


        /// <summary>
        /// Cập nhật lại file có tên tương ứng với id
        /// </summary>
        /// <param name="account"></param>
        static void UpdateFileAccount(Account account, long balance)
        {
            using (StreamWriter streamWriter = new StreamWriter($"E:\\AppATM\\{account.Id}.txt"))
                streamWriter.WriteLine($"{account.Id}#{account.Name}#{account.TypeCurrency}#{account.Balance + balance}");
        }


        /// <summary>
        /// Chức năng 3. Hiển thị giao diện của chức năng Chuyển Tiền
        /// User có thể chọn chuyển tiền, quay lại hoặc trở về menu login.
        /// </summary>
        static void DisplayTransferMoney()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔═══════════════════════════╗");
            CenterWriteLine("║        CHUYỂN TIỀN        ║");
            CenterWriteLine("╠═══════════════════════════╣");
            CenterWriteLine("║1. Tiến hành chuyển tiền   ║");
            CenterWriteLine("║B. Quay lại                ║");
            CenterWriteLine("║H. Home (Menu Login)       ║");
            CenterWriteLine("╚═══════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    TransferMoney();
                    GoBackOrHome(MenuUser, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuUser();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayTransferMoney, MenuUser, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Xử lý việc chuyển tiền của chức năng 3
        /// </summary>
        static void TransferMoney()
        {
            ViewAccount();
            Account transferAccount = ReadFileIDUser(idUser);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n- Nhập số tài khoản cần chuyển: ");
            string idToTransfer = Console.ReadLine();
            if (!CheckDestinationAccount(idToTransfer, idUser))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("- Tài khoản tồn tại hoặc không hợp lệ!");
                Console.Write("\nF5 - nhập lại, B - trở về, H - Menu Login");
                ChooseResetOrBackOrHome(DisplayTransferMoney, MenuUser, MenuLogin);
            }

            Console.Write("- Nhập số tiền cần chuyển: ");
            long amountTransfer = long.Parse(Console.ReadLine());
            if (CheckTransactionAmount(amountTransfer, transferAccount.Balance, DisplayTransferMoney))
            {
                Account destinationAccount = ReadFileIDUser(idToTransfer);
                UpdateFileAccount(transferAccount, -amountTransfer);
                UpdateFileAccount(destinationAccount, +amountTransfer);

                TransactionHistory transferHistory = new TransactionHistory(idToTransfer, "Chuyển tiền", amountTransfer, DateTime.Now);
                TransactionHistory destinationHistory = new TransactionHistory(idUser, "Nhận tiền", amountTransfer, DateTime.Now);

                WriteTransactionHistory(transferHistory, idUser);
                WriteTransactionHistory(destinationHistory, idToTransfer);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                CenterWriteLine("╔ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╗");
                CenterWriteLine("  CHUYỂN TIỀN THÀNH CÔNG   ");
                CenterWriteLine("╚ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╝");
                CenterWriteLine("H To Home - B To Menu Admin");
            }
        }


        /// <summary>
        /// Đọc file Cards và kiểm tra số tài khoản được chuyển tiền có tồn tại hay không
        /// </summary>
        /// <param name="idToTransfer">Số tài khoản được chuyển tiền</param>
        static bool CheckDestinationAccount(string idToTransfer, string idUser)
        {
            try
            {
                string[] lines = File.ReadAllLines("E:\\AppATM\\Cards.txt");
                foreach (string line in lines)
                {
                    string[] part = line.Split('#');
                    if (part.Length == 3 && part[0] == idToTransfer && idUser != idToTransfer)
                        return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return false;
        }


        /// <summary>
        /// Chức năng 4. Hiển thị giao diện của chức năng Xem Thông Tin Giao Dịch
        /// User có thể chọn xem, quay lại hoặc trở về menu login.
        /// </summary>
        static void DisplayViewTransaction()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔═══════════════════════════╗");
            CenterWriteLine("║  XEM THÔNG TIN GIAO DỊCH  ║");
            CenterWriteLine("╠═══════════════════════════╣");
            CenterWriteLine("║1. Tiến hành xem           ║");
            CenterWriteLine("║B. Quay lại                ║");
            CenterWriteLine("║H. Home (Menu Login)       ║");
            CenterWriteLine("╚═══════════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    ViewTransaction();
                    GoBackOrHome(MenuUser, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuUser();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayViewTransaction, MenuUser, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Đọc file và hiển thị thông tin giao dịch của chức năng 4
        /// </summary>
        static void ViewTransaction()
        {
            transactionList.Clear();
            try
            {
                string[] lines = File.ReadAllLines($"E:\\AppATM\\LichSu{idUser}.txt");
                for (int i = 2; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('#');
                    if (parts.Length == 4)
                    {
                        string transactionType = parts[0];
                        string id = parts[1];
                        long amount = long.Parse(parts[2]);
                        DateTime transactionTime = DateTime.Parse(parts[3]);
                        transactionList.AddLast(new TransactionHistory(id, transactionType, amount, transactionTime));
                    }
                }
                ShowTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        /// <summary>
        /// Hiển thị thông tin giao dịch
        /// </summary>
        static void ShowTransaction()
        {
            Console.ForegroundColor = ConsoleColor.White;
            CenterWriteLine("╔═════════════════════════════════════════════════════════════════════════════╗");
            CenterWriteLine("║                            Thông tin giao dịch                              ║");
            CenterWriteLine("╠══════════════════╤════════════════╤═════════════════╤═══════════════════════╣");
            CenterWriteLine("║  Loại giao dịch  │       ID       │     Số tiền     │       Thời gian       ║");
            CenterWriteLine("╟──────────────────┼────────────────┼─────────────────┼───────────────────────╢");
            foreach (var transaction in transactionList)
            {
                CenterWriteLine($"║ {transaction.TransactionType,-16} │ {transaction.Id,-14} │ {transaction.AmountMoney,-15} │ {transaction.TransactionTime,-20} ║");
            }

            CenterWriteLine("╚══════════════════╧════════════════╧═════════════════╧═══════════════════════╝");
            Console.ResetColor();
        }


        /// <summary>
        /// Chức năng 5. Hiển thị giao diện của chức năng Đổi Mật Khẩu
        /// User có thể chọn đổi mk, quay lại hoặc trở về menu login.
        /// </summary>
        static void DisplayChangePassword()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterWriteLine("╔══════════════════════╗");
            CenterWriteLine("║     ĐỔI MẬT KHẨU     ║");
            CenterWriteLine("╠══════════════════════╣");
            CenterWriteLine("║1. Đổi mật khẩu       ║");
            CenterWriteLine("║B. Quay lại           ║");
            CenterWriteLine("║H. Home (Menu Login)  ║");
            CenterWriteLine("╚══════════════════════╝");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine();
                    ChangePassword();
                    GoBackOrHome(MenuUser, MenuLogin);
                    break;
                case ConsoleKey.B:
                    MenuUser();
                    break;
                case ConsoleKey.H:
                    MenuLogin();
                    break;
                default:
                    ShowAlertInvalidSelection();
                    ChooseResetOrBackOrHome(DisplayChangePassword, MenuUser, MenuLogin);
                    break;
            }
        }


        /// <summary>
        /// Kiểm tra và thực hiện việc thay đổi mật khẩu của chức năng 5
        /// </summary>
        static void ChangePassword()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("- Nhập mật khẩu cũ: ");
            string oldPin = Console.ReadLine();
            if (!CheckOldPin(oldPin))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("- Mật khẩu không đúng!");
                Console.Write("\nF5 - nhập lại, B - trở về, H - Menu Login");
                ChooseResetOrBackOrHome(DisplayChangePassword, MenuUser, MenuLogin);
            }

            Console.Write("- Nhập mật khẩu mới: ");
            string newPin = Console.ReadLine();
            Console.Write("- Nhập lại mật khẩu mới: ");
            string newPin2 = Console.ReadLine();

            while (newPin != newPin2)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("- Mã PIN mới không khớp. Vui lòng nhập lại.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("- Nhập mật khẩu mới: ");
                newPin = Console.ReadLine();
                Console.Write("- Nhập lại mật khẩu mới: ");
                newPin2 = Console.ReadLine();
            }

            UpdatePinInFile(newPin);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            CenterWriteLine("╔ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╗");
            CenterWriteLine("  ĐỔI MẬT KHẨU THÀNH CÔNG  ");
            CenterWriteLine("╚ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ═ ╝");
            CenterWriteLine("H To Home - B To Menu Admin");
        }


        /// <summary>
        /// Cập nhật mật khẩu mới vào file Cards
        /// </summary>
        /// <param name="newPin"></param>
        static void UpdatePinInFile(string newPin)
        {
            try
            {
                // Đọc dữ liệu trong file có tên tương ứng với id
                string[] lines = File.ReadAllLines($"E:\\AppATM\\Cards.txt");
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('#');
                    if (parts.Length == 3 && parts[0] == idUser)
                    {
                        lines[i] = $"{idUser}#{newPin}#0";
                        break;
                    }
                }
                File.WriteAllLines($"E:\\AppATM\\Cards.txt", lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        /// <summary>
        /// Đọc file Cards để kiểm tra mật khẩu cũ mà người dùng nhập có đúng hay không
        /// </summary>
        /// <param name="oldPin">Mật khẩu cần kiểm tra</param>
        /// <returns></returns>
        static bool CheckOldPin(string oldPin)
        {
            try
            {
                // Đọc dữ liệu trong file có tên tương ứng với id
                string[] lines = File.ReadAllLines($"E:\\AppATM\\Cards.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split('#');
                    if (parts.Length == 3)
                    {
                        string id = parts[0];
                        string pin = parts[1];
                        int loginAttemptsCount = Convert.ToInt32(parts[2]);

                        if (oldPin == pin)
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return false;
        }



        /// <summary>
        /// Hiển thị thông báo để người dùng quay lại trang chủ (Menu Login).
        /// Yêu cầu người dùng nhấn phím "H" để quay lại trang chủ hoặc nhấn bất kỳ phím nào để tiếp tục.
        /// </summary>
        static void ReturnHomeOrContinue()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press H To Home\n-------------------------\nPress Any Key To Continue");
            Console.ResetColor();
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.H)
                MenuLogin();
        }


        /// <summary>
        /// Hiển thị thông báo khi chọn chức năng không hợp lệ.
        /// </summary>
        static void ShowAlertInvalidSelection()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            CenterWriteLine("╔════════════════════════════════════╗");
            CenterWriteLine("║   Lựa chọn không hợp lệ!           ║");
            CenterWriteLine("║   (F5: Reset - B: Back - H: Home)  ║");
            CenterWriteLine("╚════════════════════════════════════╝");
        }


        /// <summary>
        /// Hiển thị thông báo khi đăng nhập không thành công.
        /// </summary>
        static void ShowAlertWrongUsername()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            CenterWriteLine("╔═══════════════════════════════════╗");
            CenterWriteLine("║  Sai tên đăng nhập!               ║");
            CenterWriteLine("║  (F5: Reset - B: Back - H: Home)  ║");
            CenterWriteLine("╚═══════════════════════════════════╝");
        }


        /// <summary>
        /// Cập nhật lại dữ liệu trong file Cards.txt.
        /// </summary>
        static void UpdateFileCards()
        {
            string filePath = "E:\\AppATM\\Cards.txt";
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                foreach (var card in cardList)
                    streamWriter.WriteLine($"{card.Id}#{card.Pin}#{card.LoginAttemptsCount}");
            }
        }


        /// <summary>
        /// Tạo file lịch sử giao dịch.
        /// </summary>
        static void CreateTransactionFile(Account newAccount)
        {
            string filePath = $"E:\\AppATM\\LichSu{newAccount.Id}.txt";
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine($"ID: {newAccount.Id}");
                streamWriter.WriteLine($"Ngày tạo tài khoản: {DateTime.Now}");
            }
        }


        /// <summary>
        /// Tạo file tài khoản mới.
        /// </summary>
        static void CreateAccountFile(Account newAccount)
        {
            string filePath = $"E:\\AppATM\\{newAccount.Id}.txt";
            using (StreamWriter streamWriter = new StreamWriter(filePath))
                streamWriter.WriteLine($"{newAccount.Id}#{newAccount.Name}#{newAccount.TypeCurrency}#{newAccount.Balance}");
        }


        /// <summary>
        /// Kiểm tra ID có hợp lệ hay không.
        /// </summary>
        /// <param name="newID">ID cần kiểm tra</param>
        static bool IsValidID(string newID)
        {
            return Regex.IsMatch(newID, @"^\d{14}$");
        }


        /// <summary>
        /// Quay lại khi bấm phím B.
        /// Trở về MenuLogin bấm phím H.
        /// </summary>
        static void GoBackOrHome(Action backAction, Action homeAction)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.B)
                backAction();
            else if (key.Key == ConsoleKey.H)
                homeAction();
        }


        /// <summary>
        /// Mã hóa mật khẩu.
        /// </summary>
        /// <returns>Mật khẩu đã được mã hóa</returns>
        static string EncryptPassword()
        {
            string password = "";
            ConsoleKeyInfo key;
            int cursorLeft = Console.CursorLeft; // Lưu vị trí con trỏ văn bản hiện tại
            do
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1, 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }

                // Nếu phím không phải là phím Enter, thêm ký tự vào chuỗi mật khẩu và in ra màn hình
                else if (key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.SetCursorPosition(cursorLeft, Console.CursorTop);
                    Console.Write(new string('*', password.Length));
                }
            } while (key.Key != ConsoleKey.Enter);
            return password;
        }


        /// <summary>
        /// In thông tin ra giữa màn hình.
        /// </summary>
        /// <param name="text">Chuỗi cần in ra giữa</param>
        static void CenterWriteLine(string text)
        {
            Console.WriteLine("{0," + ((Console.WindowWidth / 2) + (text.Length / 2)) + "}", text);
        }


        /// <summary>
        /// In thông tin ra giữa màn hình nhưng không xuống dòng.
        /// </summary>
        /// <param name="text">Chuỗi cần in ra giữa</param>
        static void CenterWrite(string text)
        {
            Console.Write("{0," + ((Console.WindowWidth / 2.5) + (text.Length / 3)) + "}", text);
        }
    }
}
