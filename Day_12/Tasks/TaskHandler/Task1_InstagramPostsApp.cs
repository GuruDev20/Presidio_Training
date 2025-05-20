using System;

namespace Tasks.TaskHandler
{
    public class Post
    {
        public string postCaption;
        public int postLikes;
    }

    public class Task1_InstagramPostsApp
    {
        public static void Run()
        {
            Post[][] posts = ReadInstagramPosts();
            DisplayInstagramPosts(posts);
        }

        public static Post[][] ReadInstagramPosts()
        {
            int userCount = ReadPositiveInt("Enter number of users: ");
            Post[][] posts = new Post[userCount][];

            for (int i = 0; i < userCount; i++)
            {
                int postCount = ReadPositiveInt($"User {i + 1}: How many posts? ");
                posts[i] = new Post[postCount];

                for (int j = 0; j < postCount; j++)
                {
                    Console.Write($"Enter caption for post {j + 1}: ");
                    string caption;
                    do
                    {
                        caption = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(caption))
                        {
                            Console.Write("Caption cannot be empty. Enter again: ");
                        }
                    }
                    while (string.IsNullOrWhiteSpace(caption));

                    int likes = ReadNonNegativeInt("Enter likes: ");
                    posts[i][j] = new Post
                    {
                        postCaption = caption,
                        postLikes = likes
                    };
                }
            }

            return posts;
        }

        public static void DisplayInstagramPosts(Post[][] posts)
        {
            Console.WriteLine("\n--- Displaying Instagram Posts ---");
            for (int i = 0; i < posts.Length; i++)
            {
                Console.WriteLine($"User {i + 1}:");
                for (int j = 0; j < posts[i].Length; j++)
                {
                    Console.WriteLine($"Post {j + 1} - Caption: {posts[i][j].postCaption} | Likes: {posts[i][j].postLikes}");
                }
                Console.WriteLine();
            }
        }

        private static int ReadPositiveInt(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                Console.Write("Invalid input. Enter a positive number: ");
            }
            return value;
        }

        private static int ReadNonNegativeInt(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value) || value < 0)
            {
                Console.Write("Invalid input. Enter a non-negative number: ");
            }
            return value;
        }
    }
}
