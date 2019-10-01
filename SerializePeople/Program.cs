using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace SerializePeople

{
    [Serializable]
    public class Person : IDeserializationCallback, ISerializable
    {
        public enum Gender { Male, Female };
        public Gender gender;
        public string name;
        public DateTime birthDate;
        [NonSerialized] public int age;

        public Person(string name, DateTime birthDate, Gender gender)
        {
            this.name = name;
            this.birthDate = birthDate;
            this.gender = gender;
            this.age = (DateTime.Now.Year - birthDate.Year);
        }

        public Person()
        {

        }

        protected Person(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");
            name = (string)info.GetValue("AltName", typeof(string));
            gender = (Gender)info.GetValue("AltGender", typeof(int));
            birthDate = (DateTime)info.GetValue("AltBirthDate", typeof(DateTime));
        }

        public static void Serialize(string output)
        {
            Person serializablePerson = new Person(output, new DateTime(2000, 10, 10), Gender.Male);
            FileStream fs = new FileStream(output + ".dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try {
                formatter.Serialize(fs, serializablePerson);
                Console.WriteLine("Serialized {0} successfully.", output);
                }
            catch (SerializationException e)
                {
                Console.WriteLine("Failed to Serialize. Reason: {0}", e.Message);
                Console.ReadKey();
                throw;
                }
            finally
                {
                fs.Close();
                }
        }

        public static void Delete() {
            string path = Directory.GetCurrentDirectory();
            foreach (string file in Directory.GetFiles(path, "*.dat").Where(item => item.EndsWith(".dat")))
                    {
                Console.WriteLine("Deleted {0}", file);
                File.Delete(file);
            }
        }

        public static Person Deserialize(string name)
        {
            Person person = new Person();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(name + ".dat", FileMode.Open, FileAccess.Read, FileShare.Read);
            Person deserialized = (Person)formatter.Deserialize(stream);
            stream.Close();
            Console.WriteLine("Deserialized {0} successfully", name);
            return deserialized;
        }

        public void OnDeserialization(object sender)
        {
            age = (DateTime.Now.Year - birthDate.Year);
            Console.WriteLine("Deserialized: {0} who is a {1}, birth on {2} and is {3} years old.", name, gender, birthDate, age);
            Console.ReadKey();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Random random = new Random();
            if (info == null)
                throw new System.ArgumentNullException("info");
            info.AddValue("AltName", name);
            info.AddValue("AltGender", Gender.Male);
            info.AddValue("AltBirthDate", new DateTime(random.Next(1900, 2018), random.Next(1,13), random.Next(1, 29)));
        }

        public static void Main(string[] args)
        {
            Delete();
            Console.WriteLine("Enter the name of the serializable person");
            string name = Console.ReadLine();
            Serialize(name);
            Console.WriteLine("Enter x to deserialize");
            string des = Console.ReadLine();
            if (des.Equals("x"))
            {
                Deserialize(name);
            }
        }
    }
}
