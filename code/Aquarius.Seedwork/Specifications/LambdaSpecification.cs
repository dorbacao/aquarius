using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aquarius.Seedwork.Specifications
{
    public class LambdaSpecification<T> : Specification<T>
        where T : class
    {

        #region ' Constructor '

        public LambdaSpecification(Expression<Func<T, bool>> predicado)
        {
            if (predicado == null) throw new ArgumentNullException("predicado");

            this.Predicado = predicado;
        }

        #endregion

        #region ' Atributos '

        /// <summary>
        ///     Predicado.
        /// </summary>
        public Expression<Func<T, bool>> Predicado { get; set; }

        /// <summary>
        ///     cache p/ predicado compilado.
        /// </summary>
        private Func<T, bool> _predicadoCompilado;

        /// <summary>
        /// Cached compiled predicate.
        /// </summary>
        protected Func<T, bool> PredicadoCompilado
        {
            get { return _predicadoCompilado ?? (_predicadoCompilado = Predicado.Compile()); }
        }

        #endregion

        #region ' ISpecification<T> '

        public override bool SatisfiedBy(T candidate)
        {
            return PredicadoCompilado(candidate);
        }

        #endregion

        #region ' Static Methods '

        /// <summary>
        /// Combine two specifications using the AndAlso (&&) operator.
        /// </summary>
        /// <param name="leftSide">Specification that will be in the left side of the operation.</param>
        /// <param name="rightSide">Specification that will be in the left side of the operation.</param>
        /// <returns>The new combined specification.</returns>
        public static LambdaSpecification<T> operator &(LambdaSpecification<T> leftSide, LambdaSpecification<T> rightSide)
        {
            return And(leftSide, rightSide);
        }

        public static LambdaSpecification<T> And(LambdaSpecification<T> leftSide, LambdaSpecification<T> rightSide)
        {
            Expression<Func<T, bool>> left = leftSide.Predicado;

            IEnumerable<ParameterExpression> parameters = left.Parameters;

            InvocationExpression right = Expression.Invoke(rightSide.Predicado, parameters);

            BinaryExpression andAlso = Expression.AndAlso(left.Body, right);

            Expression<Func<T, bool>> newExpression = Expression.Lambda<Func<T, bool>>(andAlso, parameters);

            return new LambdaSpecification<T>(newExpression);
        }

        public LambdaSpecification<T> And(LambdaSpecification<T> other)
        {
            return And(this, other);
        }

        /// <summary>
        /// Combine two specifications using the OrElse (||) operator.
        /// </summary>
        /// <param name="leftSide">Specification that will be in the left side of the operation.</param>
        /// <param name="rightSide">Specification that will be in the left side of the operation.</param>
        /// <returns>The new combined specification.</returns>
        public static LambdaSpecification<T> operator |(LambdaSpecification<T> leftSide, LambdaSpecification<T> rightSide)
        {
            return Or(leftSide, rightSide);
        }

        public static LambdaSpecification<T> Or(LambdaSpecification<T> leftSide, LambdaSpecification<T> rightSide)
        {
            Expression<Func<T, bool>> left = leftSide.Predicado;

            IEnumerable<ParameterExpression> parameters = left.Parameters;

            InvocationExpression right = Expression.Invoke(rightSide.Predicado, parameters);

            BinaryExpression orElse = Expression.OrElse(left.Body, right);

            Expression<Func<T, bool>> newExpression = Expression.Lambda<Func<T, bool>>(orElse, parameters);

            return new LambdaSpecification<T>(newExpression);
        }

        public LambdaSpecification<T> Or(LambdaSpecification<T> other)
        {
            return Or(this, other);
        }

        #endregion

    }
}
