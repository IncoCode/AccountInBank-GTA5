namespace AccountInBank.Model
{
    internal class BankAccount
    {
        public int Balance { get; set; }
        public GTADate InterestDate { get; set; }

        public BankAccount( int balance, GTADate interestDate )
        {
            this.Balance = balance;
            this.InterestDate = interestDate;
        }
    }
}