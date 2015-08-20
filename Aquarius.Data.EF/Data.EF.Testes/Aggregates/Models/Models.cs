using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vvs.Infraestrutura.Data.EF.Testes.Aggregates.Models
{
    // Manage your company, contacts and projects.

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdAddress { get; set; }
        public virtual CompanyAddress Address { get; set; }
        public virtual ICollection<CompanyContactBase> Contacts { get; set; }
    }

    public class CompanyAddress
    {
        public int Id { get; set; }
        public String Street { get; set; }
    }

    public abstract class CompanyContactBase
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<ContactInfo> Infos { get; set; }
    }

    public class CompanyContact : CompanyContactBase
    {
    }

    public class ContactInfo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public Manager LeadCoordinator { get; set; }
        public virtual ICollection<Company> Stakeholders { get; set; }
    }

    public class Manager
    {
        // to allow for testing of multi keys and data annotations
        [Key]
        [Column(Order = 1)]
        public string PartKey { get; set; }
        [Key]
        [Column(Order = 2)]
        public int PartKey2 { get; set; }

        public string FirstName { get; set; }
        public ICollection<Project> Projects { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }

    public class Employee
    {
        // This key will be configured in fluent api
        public string Key { get; set; }
        public virtual Manager Manager { get; set; } // cyclic navigation
        public string FirstName { get; set; }
    }
}
