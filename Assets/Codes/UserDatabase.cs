using System.Collections.Generic;

[System.Serializable]
public class UserDatabase
{
    public List<User> users = new List<User>();

    public UserDatabase()
    {
        if (users == null)
        {
            users = new List<User>();
        }
    }
}