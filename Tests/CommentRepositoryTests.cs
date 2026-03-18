using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Tests;

[TestClass]
public class CommentRepositoryTests : IntegrationTestBase
{
    
    [TestMethod]
    public void AddPostWithComments_ValidPostWithComments_GettingAllComments()
    {
        //given
        var post = DataGenerator.GetPostFaker().Generate();
        _repository.AddPost(post);
        var comments = DataGenerator.GetCommentFaker(post.Id).Generate(3);
        //when
        foreach (var c in comments)
        {
            _repository.AddComment(c);
        }
        //then
        var commentsFromDatabase = _repository.GetCommentsByPostId(post.Id).ToList();
        Assert.AreEqual(3, commentsFromDatabase.Count);
        Assert.IsTrue(commentsFromDatabase.All(c => c.PostId == post.Id));
    }
    
    [TestMethod]
    public void AddComment_ValidData_IncreasesCountForPost()
    {
        //given
        var post = DataGenerator.GetPostFaker().Generate();
        _repository.AddPost(post);
        var comment = DataGenerator.GetCommentFaker(post.Id).Generate();
        //when
        _repository.AddComment(comment);
        //then
        var commentsFromDatabase = _repository.GetCommentsByPostId(post.Id).ToList();
        Assert.AreEqual(4, commentsFromDatabase.Count);
        Assert.IsTrue(commentsFromDatabase.All(c => c.PostId == post.Id));
    }
    
    [TestMethod]
    public void GetCommentsByPostId_NonExistentPost_ReturnsEmpty()
    {
        //when
        var comments = _repository.GetCommentsByPostId(99999);
        //then
        Assert.IsNotNull(comments);
        Assert.IsTrue(comments.IsNullOrEmpty());
    }
    
    [ExpectedException(typeof(DbUpdateException))]
    [TestMethod]
    public void AddComment_OrphanComment_ThrowsException()
    {
        // given
        var comment = DataGenerator.GetCommentFaker(99999).Generate();

        // when
        _repository.AddComment(comment);
    }
    
    [TestMethod]
    public void MultipleComments_DifferentPosts_ReturnsOnlyCorrectOnes()
    {
        // Given
        var posts = DataGenerator.GetPostFaker().Generate(2);

        _context.Posts.AddRange(posts);
        _context.SaveChanges();

        var commentsForFirstPost = DataGenerator.GetCommentFaker(posts[0].Id).Generate(5);
        var commentsForSecondPost = DataGenerator.GetCommentFaker(posts[1].Id).Generate(2);

        _context.Comments.AddRange(commentsForFirstPost);
        _context.Comments.AddRange(commentsForSecondPost);
        _context.SaveChanges();

        // When
        var result = _repository.GetCommentsByPostId(posts[0].Id).ToList();

        // Then
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(c => c.PostId == posts[0].Id));
    }
    
}