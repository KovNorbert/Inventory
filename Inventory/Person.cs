
namespace Inventory
{
    public  class Person
    {
        public int PersonID { get; set; }
        public string PersonName { get; set; }
        public string PersonPosition { get; set; }

        public Person(string name, string position)
        {
            PersonName = name;
            PersonPosition = position;
        }
    }


}
