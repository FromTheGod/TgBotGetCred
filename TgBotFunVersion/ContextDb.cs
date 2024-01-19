
using System.Data.Entity;
using System.Linq;


namespace TgBotFunVersion
{
    internal class ContextDb: DbContext
    {
        private static string connectionString = @"Data Source = DESKTOP-0KEOSN2;Initial Catalog = TableWithDebtors; Integrated Security = True";
        public ContextDb() : base(connectionString) { }

        public DbSet<Person> Persons {  get; set; }

        public bool ExistIdOrNot(long Id) //Проверка является ли данный Id клиентом компании
        {
            using (var context = new ContextDb())
            {
                bool ExistOrNot = context.Persons.Any(i => i.IdTelegram == Id.ToString());
                return ExistOrNot;
            }
        }
        public bool DebtOrNot(long Id) // Проверка является ли данный Id должником компании
        {
            using (var context = new ContextDb())
            {
                bool ExistOrNot = context.Persons.Where(i=>i.IdTelegram == Id.ToString()).Any(i=>i.Debt == 1);
                return ExistOrNot;
            }
        }
        public Person ReturnDebt(long Id) 
        {
            using (var context = new ContextDb())
            {
                Person needId = context.Persons.FirstOrDefault(i => i.IdTelegram == Id.ToString());
                return needId;
            }
        }
        public void AddClient(Person client) //Добавление нового клиента в базу данных
        {
            using (var context = new ContextDb())
            {
                context.Persons.Add(client);
                context.SaveChanges();
            }
        }
        public string UpdateCreditSize(long Id, decimal numberMoney) // Обновление суммы жолга после внесения дипозита
        {
            using (var context = new ContextDb())
            { 
                var update = context.Persons.FirstOrDefault(i=> i.IdTelegram == Id.ToString());
                if (update != null && numberMoney > update.SizePayment) 
                {
                    update.DatePayment = update.DatePayment.AddMonths(+1);
                    update.CreditSize = update.CreditSize - numberMoney;
                    context.SaveChanges();
                    return "Оплата прошла успешно";
                }
                else if (numberMoney < update.SizePayment) 
                {
                    return "Введенная сумма меньше минимальной месячной оплаты";
                }
                return "Пользователь не найден";
            }
        }
    }

}
