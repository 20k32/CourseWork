using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyHRD
{
    public class LogJournal
    {
        public int Id { get; set; }
        public string Initials { get; set; }
        public string Password { get; set; } 
        public string Role { get; set; }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class Cabinet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CabinetNumber { get; set; }
        public int Floor { get; set; }
        [Browsable(false)]
        public ICollection<Seats>? Seats { get; set; }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Browsable(false)]
        public ICollection<Seats>? seats { get; set; }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class Seats
    {
        public int Id { get; set; }
        public int? PostId { get; set; }
        [Browsable(false)]
        public Post? Post { get; set; }
        public byte PostCount { get; set; }
        public int? CabinetId { get; set; }
        [Browsable(false)]
        public Cabinet? Cabinet { get; set; }
        public decimal Salary { get; set; }
        [Browsable(false)]
        public ICollection<Appointments>? Appointments { get; set; }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class Appointments
    {
        public int Id { get; set; }
        [Browsable(false)]
        public Person? Person { get; set; }
        public int? PersonId { get; set; }
        public decimal Stake { get; set; }
        [Column(TypeName = "date")]
        public DateTime InvokationDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DeletionDate { get; set; }
        public string? DismissalReason { get; set; }
        public int? SeatsID { get; set; }
        [Browsable(false)]
        public Seats? Seat { get; set; }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class PersonsEducation
    {
        public int Id { get; set; }
        public string Profession { get; set; } = string.Empty;
        public string? LanguageSkills { get; set; }
        public string? ACDegree { get; set; }
        [Browsable(false)]
        public ICollection<Person>? Persons;
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class Person
    {
        public int Id { get; set; }
        public string Initials { get; set; } 
        public string Nationality { get; set; }
        [Browsable(false)]
        public PersonsEducation? Education { get; set; }
        public int? EducationId { get; set; }
        public char Sex { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        public string? FamilyStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsPhotoAvaliable { get; set; }
        public byte ChildrensCount { get; set; }
        [Browsable(false)]
        public ICollection<Appointments> ServicePlacements { get; set; }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class SimpleInformation
    {
        public int Id { get; set; }
        public string Operation { get; set; }
        public DateTime Date { get; set; }
        public string Information { get; set; }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
    }
    public class PersonInformation : SimpleInformation
    {
        public string Initials { get; set; }
    }
    public class SeatsInformation : SimpleInformation
    {
        public string Post { get; set; }
        public string Cabinet { get; set; }
    }
    public class AppointmentsInformation : SimpleInformation
    {
        public string Person { get; set; }
        public string Stake { get; set; }
    }
}
