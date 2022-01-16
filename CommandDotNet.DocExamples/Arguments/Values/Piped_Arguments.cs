using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Values
{
    [TestFixture]
    public class Piped_Arguments
    {
        public class Program
        {
            public static readonly UserService userSvc = new();

            // begin-snippet: piped_arguments
            public void List(IConsole console,
                [Option('i')] bool idsOnly)
            {
                foreach (var user in userSvc.GetUsers())
                    console.Out.WriteLine(idsOnly ? user.Id : user);
            }
            
            public void Disable(IConsole console, IEnumerable<string> ids)
            {
                foreach (string id in ids)
                {
                    if (userSvc.TryDisable(id, out var user))
                        console.Out.WriteLine($"disabled {user}");
                    else
                        console.Out.WriteLine($"user not found {id}");
                }
            }
            // end-snippet
        }
        public class Program_Options
        {
            public static readonly UserService userSvc = new();

            public void List(IConsole console,
                [Option('i')] bool idsOnly)
            {
                foreach (var user in userSvc.GetUsers())
                    console.Out.WriteLine(idsOnly ? user.Id : user);
            }

            // begin-snippet: piped_arguments_options
            public void Welcome(IConsole console, string userId, [Option] ICollection<string> notify)
            {
                console.Out.WriteLine($"welcome {userSvc.Get(userId)?.Name}");
                foreach (var user in notify
                             .Select(id => userSvc.Get(id))
                             .Where(u => u is not null && u.IsActive && u.Id != userId))
                {
                    console.Out.WriteLine($"notify: {user}");
                }
            }
            // end-snippet
        }

        public class User
        {
            public string Id { get; }
            public string Name { get; set; }
            public bool IsActive { get; set; }

            public User(string id, string name, bool isActive)
            {
                Id = id;
                Name = name;
                IsActive = isActive;
            }

            public override string ToString() => $"{Id} {Name} ({(IsActive ? "active" : "inactive")})";
        }
        public class UserService
        {
            private Dictionary<string, User> users = new List<User>
            {
                new("a1", "Avery", true), 
                new("b1", "Beatrix", true),
                new("c3", "Cris", true)
            }.ToDictionary(u => u.Id);

            public IEnumerable<User> GetUsers() =>
                users.Values.OrderBy(u => u.Id);

            public User? Get(string id) => users.GetValueOrDefault(id);

            public bool TryDisable(string id, out User? user)
            {
                if (users.TryGetValue(id, out user))
                {
                    user.IsActive = false;
                    return true;
                }

                return false;
            }
        }

        public static BashSnippet UserList = new("piped_arguments_list",
            new AppRunner<Program>(), "users.exe", "List", 0,
            @"a1 Avery (active)
b1 Beatrix (active)
c3 Cris (active)");

        public static BashSnippet UserListIdsOnly = new("piped_arguments_list_ids_only",
            new AppRunner<Program>(), "users.exe", "List -i", 0,
            @"a1
b1
c3");

        public static BashSnippet UserDisable = new("piped_arguments_disable",
            new AppRunner<Program>(), "users.exe", "Disable c3", 0,
            @"disabled c3 Cris (inactive)
disabled a1 Avery (inactive)
disabled b1 Beatrix (inactive)",
            pipedInput: (
                "users.exe list -i | grep 1 ", 
                Program.userSvc.GetUsers().Select(u => u.Id).Where(id => id != "c3").ToArray()));

        public static BashSnippet UserWelcome = new("piped_arguments_options_notify",
            new AppRunner<Program_Options>(), "users.exe", "Welcome c3 --notify ^", 0,
            @"welcome Cris
notify: a1 Avery (active)
notify: b1 Beatrix (active)",
            pipedInput: ("users.exe list -i", Program.userSvc.GetUsers().Select(u => u.Id).ToArray()));

        public static BashSnippet UserWelcomePipeToDirective = new("piped_arguments_options_notify_pipeto_directive",
            new AppRunner<Program_Options>(), "users.exe", "[pipeto:***] Welcome c3 --notify ***", 0,
            @"welcome Cris
notify: a1 Avery (active)
notify: b1 Beatrix (active)",
            pipedInput: ("users.exe list -i", Program.userSvc.GetUsers().Select(u => u.Id).ToArray()));

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}