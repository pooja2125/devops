using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagementSystem.DataAccessLayer;
using EmployeeManagementSystem.Exception;
using EmployeeManagementSystem.Entity;
using System.Text.RegularExpressions;

namespace EmployeeManagementSystem.BuissnessLayer
{
    public class EmployeeBL
    {
        
        public bool ValidateData(EmployeeEntity employee)
        {
            bool valid = true;
            string error = "";
            if (employee.EmployeeName.Length > 50 && employee.EmployeeName == string.Empty && !Regex.IsMatch(employee.EmployeeName, "/^[A-Za-z]+/ "))
            {
                valid = false;
                error = error + "\n" + "Employee name is Required. \n Employee's Name length cannot be greater than 50 charecters.";
            }

            if (employee.PhoneNumber == string.Empty)
            {
                valid = false;
                error = error + "\n Phone number is Required";
            }

            if (!Regex.IsMatch(employee.PhoneNumber, "[7-9][0-9]{9}"))
            {
                valid = false;
                error = error + "\n Phone Number should be of 10 digits and first digit must be b/w 7-9.";
            }
            if (employee.Email == string.Empty)
            {
                valid = false;
                error = error + "\n Email is required.";
            }
            if (!Regex.IsMatch(employee.Email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))

            {

                valid = false;
                error = error + "\n Email is required.";
            }

            if (employee.DateOfBirth.ToString() == string.Empty)
            {
                valid = false;
                error = error + "\n DOB is Required ";
            }

            if ((employee.DateOfBirth.Year - DateTime.Now.Year) > 18)
            {
                valid = false;
                error = error + "\n Age should be more than 18 ";
            }

            if (employee.DateOfJoining > DateTime.Now)
            {
                valid = false;
                error = error + "\n invalid date of joining ";
            }



            if (!valid)
                throw new EmployeeDataException(error);

            return valid;
        }


        EmployeeDAL employeeDAL = new EmployeeDAL();

        public bool AddEmployeeBL(EmployeeEntity em)
        {
            bool empAded = false;
            try
            {
                if(ValidateData(em))
                empAded = employeeDAL.AddEmployee(em);
                else
                    Console.WriteLine("Data Not Valid!");



            }catch(SystemException ex)
            {
                throw new EmployeeDataException(ex.Message);
            }
            return empAded;
        }

        #region DeleteEmployee
        public bool DeleteEmployeeBL(EmployeeEntity empID)
        {
           bool empDeleted = false;
            try
            {
                empDeleted = employeeDAL.DeleteEmployee(empID);

            }
            catch(SystemException ex)
            {
                throw new EmployeeDataException(ex.Message);
            }
            return empDeleted;
        }
       

        #endregion

        #region UpdateEmployee
        public bool UpdateEmployeeBL(EmployeeEntity emplID, EmployeeEntity em)
        {
            bool Updated = false;

            try
            {
                if(ValidateData(em))
                Updated = employeeDAL.UpdateEmployee(emplID,em);
                else
                {
                    Console.WriteLine("Data is Invalid");
                }
            }
            catch (SystemException ex)
            {
                throw new EmployeeDataException(ex.Message);
            }

            return Updated;
        }

     
        #endregion

        #region Search
        public EmployeeEntity SearchEmployee(int EmployeeID)
        {
            EmployeeEntity entity = employeeDAL.SearchEmployee(EmployeeID);
            return entity;
        }
        public EmployeeEntity SearchEmployee(int EmployeeID,string nameoremail)
        {
            EmployeeEntity entity = employeeDAL.SearchEmployee(EmployeeID,nameoremail);
            return entity;
        }
        public EmployeeEntity SearchEmployee(string EmpnameOrEmail)
        {
            EmployeeEntity entity = employeeDAL.SearchEmployee(EmpnameOrEmail);
            return entity;
        }
        public EmployeeEntity SearchEmployee(int EmployeeID, string name, string email)
        {
            EmployeeEntity entity = employeeDAL.SearchEmployee(EmployeeID, name, email);
            return entity;
        }
        #endregion
        public List<EmployeeEntity> GetAll()
        {
            return employeeDAL.GetEmployeesAll();
        }

    }
}
