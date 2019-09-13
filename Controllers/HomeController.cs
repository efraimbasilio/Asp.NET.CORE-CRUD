using System;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using MVC_MyCRUD.Models;


namespace MVC_MyCRUD.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Create() //The Route when Create Button Selected
        {
            return View(); // Routed  to Create Page
        }

        public IActionResult About() 
        {
            return View(); // Routed  to About Page
        }

        public IActionResult Contact() 
        {
            return View(); // Routed  to Contact Page
        }

        // ======================================================================== //
        /// <summary>
        /// To Globaly Access Connection String that reside in appsettings.json
        /// </summary>
        public IConfiguration Configuration { get; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // ======================================================================== //

        /// <summary>
        /// To Load the data from database to the index page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<Student> studentList = new List<Student>();

            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {                
                connection.Open();

                string sql = "SELECT * FROM StudentInfo";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Student student = new Student();
                        student.Id = Convert.ToInt32(dataReader["Id"]);
                        student.Name = Convert.ToString(dataReader["Name"]);
                        student.Address = Convert.ToString(dataReader["Address"]);
                        student.Age = Convert.ToInt32(dataReader["Age"]);
                        student.Allowance = Convert.ToDecimal(dataReader["Allowance"]);
                        //student.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);

                        studentList.Add(student);
                    }
                }
                connection.Close();
            }
            return View(studentList);  
        }
        
        /// <summary>
        /// To Insert data to the Database
        /// </summary>
        /// <param name="student">The parameter instansiated for a Model</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(Student student)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO StudentInfo (Name, Address, Age, Allowance) VALUES ('{student.Name}', '{student.Address}','{student.Age}','{student.Allowance}')";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return RedirectToAction("Index"); // Routed back to Index Page
            }           
        }

        ///// <summary>
        ///// To Update Data from the selected item in the Table in the the Index Page
        ///// </summary>
        ///// <param name="id">The Selected Item in the Table</param>
        ///// <returns></returns>
        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            Student student = new Student();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT * FROM StudentInfo WHERE Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        student.Id = Convert.ToInt32(dataReader["Id"]);
                        student.Name = Convert.ToString(dataReader["Name"]);
                        student.Address = Convert.ToString(dataReader["Address"]);
                        student.Age = Convert.ToInt32(dataReader["Age"]);
                        student.Allowance = Convert.ToDecimal(dataReader["Allowance"]);
                    }
                }

                connection.Close();
            }
            return View(student);
        }

        /// <summary>
        /// To update the data from the page to the database
        /// </summary>
        /// <param name="student">The parameter instansiated for a Model</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Update")]
        public IActionResult Update_Post(Student student)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE StudentInfo SET Name='{student.Name}', Address='{student.Address}', Age='{student.Age}', Allowance='{student.Allowance}' Where Id='{student.Id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// To delete the data selected in the table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM StudentInfo WHERE Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ViewBag.Result = "Operation got error:" + ex.Message;
                    }
                    connection.Close();
                }
            }
            return RedirectToAction("Index");
        }

    }      
}
