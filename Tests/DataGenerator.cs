using BlogCore.DAL.Models;
using Bogus;

namespace Tests;

public class DataGenerator
{
    public static Faker<Post> GetPostFaker()
    {
        return new Faker<Post>()
            .RuleFor(p => p.Author, f => f.Name.FullName())
            .RuleFor(p => p.Content, f => f.Lorem.Paragraph());
    }

    public static Faker<Comment> GetCommentFaker(int postId)
    {
        return new Faker<Comment>()
            .RuleFor(c => c.PostId, _ => postId)
            .RuleFor(c => c.Content, f => f.Lorem.Sentence());
    }
}