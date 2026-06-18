using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.ViewModels
{
    public class EmployeeDetails
    {
        public string? FMode { get; set; }
        public string EmployeeName { get; set; }
        public int ? EmployeeId { get; set; }
        public string Gender { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfJoining { get; set; }

        public int ApplicationUserId { get; set; }
        public string BloodGroup { get; set; }

        public string Nationality { get; set; }
        public string MartialStatus { get; set; }
        public string PhoneNumber { get; set; } 
        public string Email { get; set; }

        public string AlternateMoblieNumber { get; set; }
        public string EmergencyContactName { get; set; }

        public string EmergencyContactRelation { get; set; }

        public string EmergencyContactNumber { get; set; }
        public string EmergencyAlternateNumber { get; set; }
        public string FatherName { get; set; } 
        public string MotherName { get; set; }
        public string SpouseName { get; set; }
        public int NumberOfDependents { get; set; }
        public string AadhaarNumber { get; set; }
        public string PanNumber { get; set; }
        public string PassportNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PassportExpiryDate { get; set; }
        public string DrivingLicenseNumber { get; set; }
        public string EmployeeCode {  get; set; }
        public string ReportingHeadMailID {  get; set; }
         public int UserId {  get; set; }

        public string Designation {  get; set; }
        public CurrentAddressDetails CurrentAddress { get; set; }
        public PermanentAddressDetails PermanentAddress { get; set; }

    }

    public class CurrentAddressDetails
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }

        public string Country { get; set; }
        public String Pincode { get; set; }
        public bool IsPermanentAddress { get; set; }


    }
    public class PermanentAddressDetails
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }

        public string Country { get; set; }
        public String Pincode { get; set; }
        public bool IsPermanentAddress { get; set; }


    }
}
