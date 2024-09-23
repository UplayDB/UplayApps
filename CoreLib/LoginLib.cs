using System.Text;
using UbiServices.Records;
using static UbiServices.Public.V3;

namespace CoreLib
{
    public class LoginLib
    {
        public class CallbackLogin
        {
            public string? email;
            public LoginJson? TwoFA_LoginJson;
            public LoginArguments loginArguments;
        }
        public delegate LoginJson? Callback(CallbackLogin callbackLogin);

        public static LoginJson? CLI_Callback(CallbackLogin callbackLogin)
        {
            if (callbackLogin.TwoFA_LoginJson != null && !string.IsNullOrEmpty(callbackLogin.TwoFA_LoginJson.TwoFactorAuthenticationTicket))
            {
                Console.WriteLine("Your account has 2FA, please enter your code:");
                var code2fa = Console.ReadLine();
                if (code2fa == null)
                {
                    Console.WriteLine("Code cannot be empty!");
                    return null;
                }
                return Login_2FA_LA(callbackLogin.loginArguments, callbackLogin.TwoFA_LoginJson, code2fa);
            }
            else
            {
                return LoginCLI(out callbackLogin.loginArguments.Email!);
            }
        }

        public static LoginArguments GetLoginArguments(string[] args)
        {
            return new LoginArguments()
            { 
                AccountId = ParameterLib.GetParameter<string>(args, "-accid", string.Empty),
                Base64Login = ParameterLib.GetParameter<string>(args, "-b64", string.Empty),
                Email = ParameterLib.GetParameter<string>(args, "-username", string.Empty) ?? ParameterLib.GetParameter<string>(args, "-user", string.Empty) ?? ParameterLib.GetParameter<string>(args, "-email", string.Empty),
                Password = ParameterLib.GetParameter<string>(args, "-password", string.Empty) ?? ParameterLib.GetParameter<string>(args, "-pass", string.Empty),
                rememberDevice = ParameterLib.HasParameter(args, "-remember-device"),
                rememberMe = ParameterLib.HasParameter(args, "-remember-me"),
                trustedDeviceId = ParameterLib.GetParameter<string>(args, "-trustedid", string.Empty),
                trustedDeviceName = ParameterLib.GetParameter<string>(args, "-trustedname", string.Empty),
                useFileStorage = ParameterLib.HasParameter(args, "-usefilestore"),
                printStorage = ParameterLib.HasParameter(args, "-printstore"),
            };
        }

        public static LoginJson? LoginArgs_CLI(string[] args)
        {
            return LoginArgs(GetLoginArguments(args), CLI_Callback);
        }

        public static LoginJson? LoginArgs_CLI(LoginArguments loginArguments)
        {
            return LoginArgs(loginArguments, CLI_Callback);
        }

        public static LoginJson? LoginArgs(LoginArguments loginArguments, Callback? callback = null)
        {
            if (loginArguments.useFileStorage)
                UbisoftLoginStore.UseIsolatedStorage = false;
            UbisoftLoginStore.LoadFromFile("LoginStore.dat");
            if (loginArguments.printStorage)
            {
                Console.WriteLine(UbisoftLoginStore.Instance.UserDatCache.ToString());
                Console.WriteLine(UbisoftLoginStore.Instance.RememberCache.ToString());
            }
            LoginJson? login = null;
            if (!string.IsNullOrEmpty(loginArguments.Base64Login))
            {
                login = LoginBase64(loginArguments.Base64Login);
            }
            else if (loginArguments.rememberMe && loginArguments.rememberDevice && !string.IsNullOrEmpty(loginArguments.AccountId))
            {
                login = Login_AccIdRemember_Device(loginArguments);
            }
            else if (loginArguments.rememberMe && !string.IsNullOrEmpty(loginArguments.AccountId))
            {
                login = Login_AccIdRemember(loginArguments);
            }
            else if(loginArguments.rememberMe && !string.IsNullOrEmpty(loginArguments.Email))
            {
                var userdata = UbisoftLoginStore.Instance.UserDatCache.Prod.Users.Where(x => x.Email == loginArguments.Email).FirstOrDefault();
                if (userdata != null && !string.IsNullOrEmpty(userdata.RememberMeTicket))
                {
                    login = LoginRemember(userdata.RememberMeTicket);
                }
                else
                {
                    login = callback?.Invoke(new CallbackLogin()
                    {
                        email = loginArguments.Email,
                        loginArguments = loginArguments
                    });
                }
            }
            else if(!string.IsNullOrEmpty(loginArguments.Email) && !string.IsNullOrEmpty(loginArguments.Password))
            {
                login = Login(loginArguments.Email, loginArguments.Password);
            }
            else
            {
                var cblogin = new CallbackLogin()
                {
                    email = loginArguments.Email,
                    TwoFA_LoginJson = login,
                    loginArguments = loginArguments
                };
                login = callback?.Invoke(cblogin);
                loginArguments.Email = cblogin.email;
            }
            if (login == null)
            {
                var cblogin = new CallbackLogin()
                {
                    email = loginArguments.Email,
                    TwoFA_LoginJson = login,
                    loginArguments = loginArguments
                };
                login = callback?.Invoke(cblogin);
                loginArguments.Email = cblogin.email;
            }
            if (login != null && login.Ticket == null && login.TwoFactorAuthenticationTicket != null)
            {
                var cblogin = new CallbackLogin()
                {
                    email = loginArguments.Email,
                    TwoFA_LoginJson = login,
                    loginArguments = loginArguments
                };
                login = callback?.Invoke(cblogin);
                loginArguments.Email = cblogin.email;
            }
            if (login != null)
            {
                var user = UbisoftLoginStore.Instance.UserDatCache.Prod.Users.Where(x => x.UbiProfileId == login.ProfileId).FirstOrDefault();
                if (user == null)
                {
                    user = new Uplay.UserDatFile.UserInfo()
                    {
                        Email = loginArguments.Email,
                        Name = login.NameOnPlatform,
                        RememberMeTicket = login.RememberMeTicket,
                        Username = login.NameOnPlatform,
                        UbiAccountId = login.UserId,
                        UbiProfileId = login.ProfileId,
                    };
                }
                else
                {
                    UbisoftLoginStore.Instance.UserDatCache.Prod.Users.Remove(user);
                    user.RememberMeTicket = login.RememberMeTicket;
                    user.Name = login.NameOnPlatform;
                }
                UbisoftLoginStore.Instance.UserDatCache.Prod.Users.Add(user);
                UbisoftLoginStore.Save();
            }
            return login;
        }


        static LoginJson? Login_AccIdRemember(LoginArguments loginArguments)
        {
            var userdata = UbisoftLoginStore.Instance.UserDatCache.Prod.Users.Where(x => x.UbiAccountId == loginArguments.AccountId).FirstOrDefault();
            if (userdata != null && !string.IsNullOrEmpty(userdata.RememberMeTicket))
            {
                return LoginRemember(userdata.RememberMeTicket);
            }
            else if (!string.IsNullOrEmpty(loginArguments.Email) && !string.IsNullOrEmpty(loginArguments.Password))
            {
                return Login(loginArguments.Email, loginArguments.Password);
            }
            return null;
        }

        static LoginJson? Login_AccIdRemember_Device(LoginArguments loginArguments)
        {
            var userdata = UbisoftLoginStore.Instance.UserDatCache.Prod.Users.Where(x => x.UbiAccountId == loginArguments.AccountId).FirstOrDefault();
            var rem = UbisoftLoginStore.Instance.RememberCache.Users.Where(x => x.AccountId == loginArguments.AccountId).FirstOrDefault();
            if (userdata != null && !string.IsNullOrEmpty(userdata.RememberMeTicket) && rem != null && !string.IsNullOrEmpty(rem.RdTicket))
            {
                return LoginRememberDevice(userdata.RememberMeTicket, rem.RdTicket);
            }
            else
                return Login_AccIdRemember(loginArguments);
        }

        static LoginJson? Login_2FA_LA(LoginArguments loginArguments, LoginJson loginJson, string code2fa)
        {
            if (string.IsNullOrEmpty(code2fa))
                return null;
            if (loginArguments.rememberDevice)
            {
                string trustedname = (!string.IsNullOrEmpty(loginArguments.trustedDeviceName)) ? loginArguments.trustedDeviceName : Environment.MachineName;
                string trustedid = (!string.IsNullOrEmpty(loginArguments.trustedDeviceId)) ? loginArguments.trustedDeviceId : GenerateDeviceId(trustedname);
                return TryLoginWith2FA_Rem(loginJson, code2fa, trustedname, trustedid);
            }
            else
            {
                return TryLoginWith2FA(loginJson, code2fa);
            }
        }


        public static LoginJson? LoginCLI(out string? email)
        {
            Console.WriteLine("Please enter your Email:");
            email = Console.ReadLine()!;
            Console.WriteLine("Please enter your Password:");
            string password = ReadPassword();
            return Login(email, password);
        }


        //  2fa
        public static LoginJson? TryLoginWith2FA_Rem(LoginJson? login, string code2fa, string trustedname)
        {
            var deviceId = GenerateDeviceId(trustedname);
            return TryLoginWith2FA_Rem(login, code2fa, trustedname, deviceId);
        }

        public static LoginJson? TryLoginWith2FA_Rem(LoginJson? login, string code2fa, string trustedname, string trustedId)
        {
            LoginJson? ret = login;
            if (login != null && login.Ticket == null && login.TwoFactorAuthenticationTicket != null)
            {
                ret = Login2FA_Device(login.TwoFactorAuthenticationTicket, code2fa, trustedId,trustedname);

                if (ret != null)
                {
                    var rem = UbisoftLoginStore.Instance.RememberCache.Users.Where(x => x.AccountId == ret.UserId).FirstOrDefault();
                    rem ??= new()
                        {
                            AccountId = ret.UserId
                        };
                    rem.RdTicket = ret.RememberDeviceTicket;
                }
            }
            return ret;
        }

        public static LoginJson? TryLoginWith2FA(LoginJson? login, string code2fa)
        {
            LoginJson? ret = login;
            if (login != null && login.Ticket == null && login.TwoFactorAuthenticationTicket != null)
            {
                ret = Login2FA(login.TwoFactorAuthenticationTicket, code2fa);
            }
            return ret;
        }

        //  Thanks from SteamRE!
        public static string ReadPassword()
        {
            ConsoleKeyInfo keyInfo;
            var password = new StringBuilder();

            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
                    }

                    continue;
                }
                /* Printable ASCII characters only */
                var c = keyInfo.KeyChar;
                if (c >= ' ' && c <= '~')
                {
                    password.Append(c);
                    Console.Write('*');
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            return password.ToString();
        }
    }
}