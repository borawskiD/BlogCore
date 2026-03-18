using BlogCore.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests;

[TestClass]
public class ValidationTests : IntegrationTestBase
{
    [TestMethod]
    [ExpectedException(typeof(DbUpdateException))]
    public void AddPost_NullAuthor_ThrowsDbUpdateException()
    {
        //given
        var postWithoutAuthor = new Post
        {
            Author = null,
            Content = "Hello world!"
        };
        //when
        _repository.AddPost(postWithoutAuthor);
    }
    
    [TestMethod]
    [ExpectedException(typeof(DbUpdateException))]
    public void AddComment_NullContent_ThrowsDbUpdateException()
    {
        //given
        var post = new Post
        {
            Author = "Tester",
            Content = "Hello world!"
        };
        _repository.AddPost(post);
        var commentWithoutContent = new Comment()
        {
            PostId = post.Id,
            Content = null
        };
        //when
        _repository.AddComment(commentWithoutContent);
    }
    
    
    
}