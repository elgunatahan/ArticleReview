using Domain.Repositories;
using MediatR;

namespace Application.Commands.Article.Create
{
    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CreateArticleRepresentation>
    {
        private readonly IArticleRepository _articleRepository;

        public CreateArticleCommandHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<CreateArticleRepresentation> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            var article = new Domain.Entities.Article(request.ArticleContent, request.Author, request.PublishDate, request.StarCount, request.Title);

            await _articleRepository.CreateAsync(article, cancellationToken);

            return new CreateArticleRepresentation()
            {
                Id = article.IdentityObject.Id
            };
        }
    }
}
