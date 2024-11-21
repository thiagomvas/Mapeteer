using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mapeteer;
public interface IMappingExpressionBuilder<TSource, TDestination>
{
    IMappingExpressionBuilder<TSource, TDestination> WithTransform(Action<TSource, TDestination> transform);
}
