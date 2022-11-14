﻿using Sat.Recruitment.Business.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Sat.Recruitment.Entities;
using System.IO;

namespace Sat.Recruitment.Business
{
    /// <summary>
    /// This class is in charge of doing the operations for the users.
    /// </summary>
    public class UserBsStrategy : IUserBsStrategy
    {
        private readonly List<User> _users = new List<User>();

        //Validate errors
        public void ValidateErrors(string name, string email, string address, string phone, ref string errors)
        {
            if (name == null)
                //Validate if Name is null
                errors = "The name is required";
            if (email == null)
                //Validate if Email is null
                errors = errors + " The email is required";
            if (address == null)
                //Validate if Address is null
                errors = errors + " The address is required";
            if (phone == null)
                //Validate if Phone is null
                errors = errors + " The phone is required";
        }


        public Result Calculate(string name, string email, string address, string phone, string userType, string money, string errors)
        {
            Result result_ = new Result();

            try
            {
                if (errors != null && errors != "")
                {
                    result_.Errors = errors;
                    result_.IsSuccess = false;
                }
                else
                {
                    var newUser = new User
                    {
                        Name = name,
                        Email = email,
                        Address = address,
                        Phone = phone,
                        UserType = userType,
                        Money = decimal.Parse(money)
                    };

                    if (newUser.UserType == "Normal")
                    {
                        if (decimal.Parse(money) > 100)
                        {
                            var percentage = Convert.ToDecimal(0.12);
                            //If new user is normal and has more than USD100
                            var gif = decimal.Parse(money) * percentage;
                            newUser.Money = newUser.Money + gif;
                        }
                        if (decimal.Parse(money) < 100)
                        {
                            if (decimal.Parse(money) > 10)
                            {
                                var percentage = Convert.ToDecimal(0.8);
                                var gif = decimal.Parse(money) * percentage;
                                newUser.Money = newUser.Money + gif;
                            }
                        }
                    }
                    if (newUser.UserType == "SuperUser")
                    {
                        if (decimal.Parse(money) > 100)
                        {
                            var percentage = Convert.ToDecimal(0.20);
                            var gif = decimal.Parse(money) * percentage;
                            newUser.Money = newUser.Money + gif;
                        }
                    }
                    if (newUser.UserType == "Premium")
                    {
                        if (decimal.Parse(money) > 100)
                        {
                            var gif = decimal.Parse(money) * 2;
                            newUser.Money = newUser.Money + gif;
                        }
                    }

                    result_ = checkUser(newUser);
                }
            }
            catch (Exception ex)
            {

                result_.Errors = ex.ToString();
                result_.IsSuccess = false;
            }

            return result_;

        }

        private StreamReader ReadUsersFromFile()
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";

            FileStream fileStream = new FileStream(path, FileMode.Open);

            StreamReader reader = new StreamReader(fileStream);
            return reader;
        }


        private Result checkUser(User newUser) {

            var reader = ReadUsersFromFile();

            //Normalize email
            var aux = newUser.Email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            newUser.Email = string.Join("@", new string[] { aux[0], aux[1] });

            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLineAsync().Result;
                var user = new User
                {
                    Name = line.Split(',')[0].ToString(),
                    Email = line.Split(',')[1].ToString(),
                    Phone = line.Split(',')[2].ToString(),
                    Address = line.Split(',')[3].ToString(),
                    UserType = line.Split(',')[4].ToString(),
                    Money = decimal.Parse(line.Split(',')[5].ToString()),
                };
                _users.Add(user);
            }
            reader.Close();
            try
            {
                var isDuplicated = false;
                foreach (var user in _users)
                {
                    if (user.Email == newUser.Email
                        ||
                        user.Phone == newUser.Phone)
                    {
                        isDuplicated = true;
                    }
                    else if (user.Name == newUser.Name)
                    {
                        if (user.Address == newUser.Address)
                        {
                            isDuplicated = true;
                            throw new Exception("User is duplicated");
                        }

                    }
                }

                if (!isDuplicated)
                {
                    Debug.WriteLine("User Created");

                    return new Result()
                    {
                        IsSuccess = true,
                        Errors = "User Created"
                    };
                }
                else
                {
                    Debug.WriteLine("The user is duplicated");

                    return new Result()
                    {
                        IsSuccess = false,
                        Errors = "The user is duplicated"
                    };
                }
            }
            catch
            {
                Debug.WriteLine("The user is duplicated");
                return new Result()
                {
                    IsSuccess = false,
                    Errors = "The user is duplicated"
                };
            }

            return new Result()
            {
                IsSuccess = true,
                Errors = "User Created"
            };
        }
    

        }
    }

