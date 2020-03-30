using System;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public decimal Wallet { get; set; }

        public char MaritalStatus { get; set; }

        public short Height { get; set; }
    }
}