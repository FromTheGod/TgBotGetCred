using System;
using System.Text.RegularExpressions;
namespace TgBotFunVersion
{
    internal class RegexForCheck
    {
        public string CheckMoney(string money)
        {
            string pattern = @"^\d+$";
            if (Regex.IsMatch(money, pattern))
            {
                return money;
            }
            else
            {
                return "Не коректный ввод денежной суммы";
            }
        }
        public string CheckDate(string date)
        {
            string pattern = @"^(0[1-9]|[12][0-9]|3[01]).(0[1-9]|1[0-2]).(19|20)\d\d$";
            if (Regex.IsMatch(date, pattern))
            {
                return date;
            }
            else
            {
                return "Не корректный ввод даты рождения";
            }
        }
        public string CheckName(string name)
        {
            string pattern = @"^[а-яА-Я]+$";
            if (Regex.IsMatch(name,pattern))
                {
                    return name;
                }
            else
                {
                    return "Не корректный ввод";
                }
        }
        public string CheckNumber(string number)
        {
            if (number.Length > 12)
            {
                return "Не корректный ввод номера";
            }
            else
            {
                string pattern = @"^8\d{10}$";
                if (Regex.IsMatch(number,pattern))
                {
                        return number;
                }
                else
                {
                    return "Не корректный ввод номера";
                }
            }
        }
        public string CheckMounth(string mounthNumber)
        {
              if (Int32.Parse(mounthNumber) < 60)
            {
                return "Срок кредита не может превышать 5 лет";
            }
              else
            {
                return mounthNumber;
            }
        }
    }
}
