using Application.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Article.Update
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand>
    {
        private readonly IArticleRepository _articleRepository;

        public UpdateArticleCommandHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Article article = await _articleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (article == null)
            {
                throw new ArticleNotFoundException(request.Id);
            }

            article.Update(request.ArticleContent, request.Author, request.PublishDate, request.StarCount, request.Title);

            await _articleRepository.UpdateAsync(article, cancellationToken);
        }
    }
}
