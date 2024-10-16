using Application.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Article.Delete
{
    public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand>
    {
        private readonly IArticleRepository _articleRepository;

        public DeleteArticleCommandHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Article article = await _articleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (article == null)
            {
                throw new ArticleNotFoundException(request.Id);
            }

            article.Delete();

            await _articleRepository.UpdateAsync(article, cancellationToken);
        }
    }
}
