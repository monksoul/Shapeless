// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Core.Extensions;

/// <summary>
///     <see cref="Type" /> 拓展类
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    ///     创建实例属性值访问器
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="propertyInfo">
    ///     <see cref="PropertyInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Func{T1, T2}" />
    /// </returns>
    internal static Func<object, object?> CreatePropertyGetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_获取_属性名
        var dynamicMethod = new DynamicMethod(
            $"{type.FullName}_Get_{propertyInfo.Name}",
            typeof(object),
            [typeof(object)],
            typeof(TypeExtensions).Module,
            true
        );

        // 获取动态方法的 IL 生成器
        var ilGenerator = dynamicMethod.GetILGenerator();

        // 获取属性的获取方法，并允许非公开访问
        var getMethod = propertyInfo.GetGetMethod(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(getMethod);

        // 将目标对象加载到堆栈上
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);

        // 调用获取方法
        ilGenerator.EmitCall(OpCodes.Callvirt, getMethod, null);

        // 如果属性类型为值类型，则装箱为 object 类型
        if (propertyInfo.PropertyType.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
        }

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Func 类型
        return (Func<object, object?>)dynamicMethod.CreateDelegate(typeof(Func<object, object?>));
    }
}