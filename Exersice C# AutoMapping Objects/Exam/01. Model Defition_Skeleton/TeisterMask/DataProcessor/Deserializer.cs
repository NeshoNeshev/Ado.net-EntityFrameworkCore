using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TeisterMask.Data.Models;
using TeisterMask.Data.Models.Enums;
using TeisterMask.DataProcessor.ImportDto;

namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;

    using Data;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportProjectDto[]), new XmlRootAttribute("Projects"));


            using (StringReader reader = new StringReader(xmlString))
            {
                var importProjectDtos = (ImportProjectDto[])xmlSerializer.Deserialize(reader);
                List<Project> projectsToAdd = new List<Project>();

                foreach (var itemDto in importProjectDtos)
                {

                    if (!IsValid(itemDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    DateTime openDate;
                    bool isDateValid = DateTime.TryParseExact(itemDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out openDate);

                    if (!isDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime? dueDate;
                    if (!String.IsNullOrEmpty(itemDto.DueDate))
                    {
                        DateTime projectDueDate;
                        bool isValidRelDate = DateTime.TryParseExact(itemDto.DueDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out projectDueDate);

                        if (!isValidRelDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        dueDate = projectDueDate;
                    }
                    else
                    {
                        dueDate = null;
                    }


                    Project project = new Project()
                    {
                        Name = itemDto.Name,
                        OpenDate = openDate,
                        DueDate = dueDate
                    };

                    foreach (var taskDto in itemDto.Tasks)
                    {


                        if (!IsValid(taskDto))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        DateTime openTaskDate;
                        bool isValidTaskOpdenDate = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out openTaskDate);

                        if (!isValidTaskOpdenDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (openTaskDate < project.OpenDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        DateTime dueTaskDate;
                        bool isValidTaskDueDate = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out dueTaskDate);

                        if (!isValidTaskDueDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (project.DueDate.HasValue)
                        {
                            if (dueTaskDate > project.DueDate)
                            {
                                sb.AppendLine(ErrorMessage);
                                continue;
                            }
                        }


                        Task task = new Task()
                        {
                            Name = taskDto.Name,
                            OpenDate = openTaskDate,
                            DueDate = dueTaskDate,
                            ExecutionType = (ExecutionType)taskDto.ExecutionType,
                            LabelType = (LabelType)taskDto.LabelType
                        };
                        project.Tasks.Add(task);

                    }

                    sb.AppendLine($"Successfully imported project - {project.Name} with {project.Tasks.Count} tasks.");
                    projectsToAdd.Add(project);
                }
                context.Projects.AddRange(projectsToAdd);
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var employeesDtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            List<Employee> employees = new List<Employee>();

            foreach (var employeeDto in employeesDtos)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (!IsValidUserName(employeeDto.Username))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee()
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone
                };


                foreach (var taskDto in employeeDto.Tasks.Distinct())
                {


                    Task task = context.Tasks.FirstOrDefault(x => x.Id == taskDto);
                    if (task == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    employee.EmployeesTasks.Add(new EmployeeTask()
                    {
                        Employee = employee,
                        Task = task
                    });

                }
                employees.Add(employee);
                sb.AppendLine(
                    $"Successfully imported employee - {employee.Username} with {employee.EmployeesTasks.Count} tasks.");
            }
            context.Employees.AddRange(employees);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
        private static bool IsValidUserName(string username)
        {
            foreach (char ch in username)
            {
                if (!char.IsLetterOrDigit(ch))
                {
                    return false;
                }
            }

            return true;
        }
    }
}