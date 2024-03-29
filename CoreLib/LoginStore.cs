﻿using Newtonsoft.Json;
using UbiServices.Records;

namespace CoreLib
{
    internal class LoginStore
    {
        public class Stored
        {
            public string UserId { get; set; }
            public string RemTicket { get; set; }
            public string RemDeviceTicket { get; set; }
        }

        public static List<Stored> Load()
        {
            List<Stored> logins = new();
            if (File.Exists(".LoginStore"))
            {
                logins = JsonConvert.DeserializeObject<List<Stored>>(File.ReadAllText(".LoginStore"));
                if (logins == null)
                    logins = new();
            }
            return logins;
        }

        public static void FromLogin(LoginJson login)
        {
            var logins = Load();
            var listUser = logins.Where(x => x.UserId == login.UserId).ToList();
            if (listUser.Count != 0)
            {
                var user = listUser.First();
                user.RemDeviceTicket = login.RememberDeviceTicket;
                user.RemTicket = login.RememberMeTicket;
            }
            else
            {
                listUser.Add(new()
                {
                    UserId = login.UserId,
                    RemDeviceTicket = login.RememberDeviceTicket,
                    RemTicket = login.RememberMeTicket
                });
            }
            Save(listUser);
        }

        public static void Save(List<Stored> stored)
        {
            File.WriteAllText(".LoginStore", JsonConvert.SerializeObject(stored));
        }
    }
}
