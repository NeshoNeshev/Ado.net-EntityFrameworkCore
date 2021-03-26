using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SoftJail.Data.Models;
using SoftJail.Data.Models.Enums;
using SoftJail.DataProcessor.ImportDto;

namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        private const string SuccessMessage = "Imported {0} with {1} cells";
        private const string SuccessImportPrisoners = "Imported {0} {1} years old";
        private const string SuccessImportOfficer = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportDepartmentDto[] departments = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);
            List<Department> departmentsToAdd = new List<Department>();


            foreach (var department in departments)
            {
                bool validate = true;


                if (!IsValid(department))
                {
                    validate = false;
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (department.Cells.Length == 0)
                {
                    validate = false;
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Department dp = new Department()
                {
                    Name = department.Name,

                };


                foreach (var item in department.Cells)
                {

                    if (!IsValid(item))
                    {
                        validate = false;
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    Cell cell = new Cell()
                    {
                        CellNumber = item.CellNumber,
                        HasWindow = item.HasWindow
                    };

                    dp.Cells.Add(cell);

                }

                if (validate == true)
                {
                    departmentsToAdd.Add(dp);
                    sb.AppendLine(string.Format(SuccessMessage, dp.Name, dp.Cells.Count));
                }

                validate = true;
            }

            context.Departments.AddRange(departmentsToAdd);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportPrisonersDto[] prisonersDtos = JsonConvert.DeserializeObject<ImportPrisonersDto[]>(jsonString);
            List<Prisoner> prisonersToAdd = new List<Prisoner>();
            foreach (var prisoners in prisonersDtos)
            {
               
                if (!IsValid(prisoners))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? releaseDate;
                if (!String.IsNullOrEmpty(prisoners.ReleaseDate))
                {
                    DateTime prisonerReleaseDate;
                    bool isValidRelDate = DateTime.TryParseExact(prisoners.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out prisonerReleaseDate);

                    if (!isValidRelDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    releaseDate = prisonerReleaseDate;
                }
                else
                {
                    releaseDate = null;
                }

                DateTime incarcerationDate;
                bool date = DateTime.TryParseExact(prisoners.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out incarcerationDate);
                if (!date)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Prisoner prisoner = new Prisoner()
                {

                    FullName = prisoners.FullName,
                    Nickname = prisoners.Nickname,
                    Age = prisoners.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    CellId = prisoners.CellId
                };
                foreach (var itemDto in prisoners.Mails)
                {
                    if (!IsValid(itemDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    Mail mail = new Mail()
                    {
                        Description = itemDto.Description,
                        Sender = itemDto.Sender,
                        Address = itemDto.Address
                    };
                    prisoner.Mails.Add(mail);
                }

                prisonersToAdd.Add(prisoner);
                sb.AppendLine(string.Format(SuccessImportPrisoners, prisoner.FullName, prisoner.Age));

            }

            context.Prisoners.AddRange(prisonersToAdd);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportOfficerDto[]), new XmlRootAttribute("Officers"));


            using (StringReader reader = new StringReader(xmlString))
            {
                ImportOfficerDto[] officerDtos = (ImportOfficerDto[]) xmlSerializer.Deserialize(reader);
                List<Officer> officers = new List<Officer>();


                foreach (var officerDto in officerDtos)
                {
                    if (!IsValid(officerDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Position position;
                    bool isValidPosition = Enum.TryParse(officerDto.Position, out position);
                    if (!isValidPosition)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Weapon weapon;
                    bool isValidWeapon = Enum.TryParse(officerDto.Weapon, out weapon);
                    if (!isValidWeapon)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    Officer officer = new Officer()
                    {
                        FullName = officerDto.FullName,
                        Salary = officerDto.Salary,
                        Position = position,
                        Weapon = weapon,
                        DepartmentId = officerDto.DepartmentId
                    };
                    foreach (var prisoner in officerDto.Prisoners)
                    {
                        officer.OfficerPrisoners.Add(new OfficerPrisoner()
                        {
                            Officer = officer,
                            PrisonerId = prisoner.Id
                        });
                    }
                    officers.Add(officer);
                    sb.AppendLine(string.Format(SuccessImportOfficer, officer.FullName,
                        officer.OfficerPrisoners.Count));
                }
                context.Officers.AddRange(officers);
                context.SaveChanges();
                
                return sb.ToString().Trim();
            }
            
        }
        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}