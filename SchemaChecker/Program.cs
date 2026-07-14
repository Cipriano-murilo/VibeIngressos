using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=db.atwtyqfnrzyedhwapmav.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=U2hTy2XRbC6144bj;SSL Mode=Require;Trust Server Certificate=true";
        using var conn = new NpgsqlConnection(connString);
        conn.Open();

        try {
            using var cmd = new NpgsqlCommand("ALTER TABLE profiles DROP CONSTRAINT profiles_id_fkey;", conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Dropped foreign key profiles_id_fkey from profiles");
        } catch(Exception e) { Console.WriteLine("Drop FK: " + e.Message); }
    }
}
