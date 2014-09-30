namespace SysWaterRev.BusinessLayer.Models
{
    public class SimpleRevCollectionRoles
    {
        public const string Customers = "Customers";
        public const string Administrators = "Administrators";
        public const string FieldOperatives = "FieldOperatives";
        public const string Employees = "Employees";
        public static readonly string[] AllRoles = {Customers, Administrators, FieldOperatives, Employees};
        public static readonly string[] EmployeeRoles = {FieldOperatives, Employees, Administrators};
        public static readonly string[] AllEmployeeRoles = {FieldOperatives, Employees, Administrators};
    }
}