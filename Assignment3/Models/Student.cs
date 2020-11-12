using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Assignment3.Models
{

   

    public class student
    {
        public String StudentId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }

        private string _DateOfBirth;

        
        public string DateOfBirth
        {
            get { return _DateOfBirth; }
            set
            {
                _DateOfBirth = value;
                
                DateTime dtOut;
                DateTime.TryParse(_DateOfBirth, out dtOut);
                DateOfBirthDT = dtOut;
            }
        }

        public String ImageData { get; set; }
        public bool MyRecord { get; set; }

        public virtual int Age
        {
            get
            {
                DateTime Now = DateTime.Now;
                int Years = new DateTime(DateTime.Now.Subtract(DateOfBirthDT).Ticks).Year - 1;
                DateTime PastYearDate = DateOfBirthDT.AddYears(Years);
                int Months = 0;
                for (int i = 1; i <= 12; i++)
                {
                    if (PastYearDate.AddMonths(i) == Now)
                    {
                        Months = i;
                        break;
                    }
                    else if (PastYearDate.AddMonths(i) >= Now)
                    {
                        Months = i - 1;
                        break;
                    }
                }
                int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
                int Hours = Now.Subtract(PastYearDate).Hours;
                int Minutes = Now.Subtract(PastYearDate).Minutes;
                int Seconds = Now.Subtract(PastYearDate).Seconds;
                return Years;
            }
        }

        public DateTime DateOfBirthDT { get; set; }

        public override string ToString()
        {
            return $"{StudentId} {FirstName} {LastName}";
        }

        public string ToCSV()
        {
            return $"{StudentId},{FirstName},{LastName}";
        }
        
    

    
    }
    public class FTP
    {
        public const string UserName = @"bdat100119f\bdat1001";
        public const string Password = "bdat1001";

        public const string BaseUrl = "ftp://waws-prod-dm1-127.ftp.azurewebsites.windows.net/bdat1001-10983";

        public const int OperationPauseTime = 10000;
    }

}