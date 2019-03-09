using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.QuickInfo;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

namespace CoCo.Services
{
    using CoCoImageKind = CoCo.Analyser.QuickInfo.ImageKind;
    using MsQuickInfoItem = Microsoft.VisualStudio.Language.Intellisense.QuickInfoItem;

    internal sealed class QuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;

        public QuickInfoSource(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
        }

        public void Dispose()
        {
        }

        public async Task<MsQuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(_textBuffer.CurrentSnapshot);
            if (!triggerPoint.HasValue) return null;

            var quickInfo = await QuickInfoService.GetQuickInfo(_textBuffer, session, cancellationToken);
            if (quickInfo is null || quickInfo.Descriptions.Length == 0) return null;

            var trackingSpan = triggerPoint.Value.Snapshot.CreateTrackingSpan(quickInfo.GetSpan(), SpanTrackingMode.EdgeInclusive);

            var items = new List<object>();
            foreach (var item in quickInfo.Descriptions)
            {
                if (item.Kind == SymbolDescriptionKind.Main)
                {
                    var containerItem = TryGetImageElement(quickInfo.Image, out var image)
                        ? new ContainerElement(ContainerElementStyle.Wrapped, image, ToClassifiedTextElement(item))
                        : new ContainerElement(ContainerElementStyle.Wrapped, ToClassifiedTextElement(item));

                    items.Insert(0, containerItem);
                }
                else
                {
                    items.Add(ToClassifiedTextElement(item));
                }
            }

            return new MsQuickInfoItem(trackingSpan, new ContainerElement(ContainerElementStyle.Stacked, items));
        }

        private static ClassifiedTextElement ToClassifiedTextElement(SymbolDescription symbolDescription)
        {
            var list = new List<ClassifiedTextRun>();
            foreach (var item in symbolDescription.TaggedParts)
            {
                list.Add(new ClassifiedTextRun(item.Tag, item.Text));
            }
            return new ClassifiedTextElement(list);
        }

        private static bool TryGetImageElement(CoCoImageKind image, out ImageElement imageElement)
        {
            var id =
                image == CoCoImageKind.ClassPublic ? KnownImageIds.ClassPublic :
                image == CoCoImageKind.ClassInternal ? KnownImageIds.ClassInternal :
                image == CoCoImageKind.ClassProtected ? KnownImageIds.ClassProtected :
                image == CoCoImageKind.ClassPrivate ? KnownImageIds.ClassPrivate :

                image == CoCoImageKind.ConstPublic ? KnownImageIds.ConstantPublic :
                image == CoCoImageKind.ConstInternal ? KnownImageIds.ConstantInternal :
                image == CoCoImageKind.ConstProtected ? KnownImageIds.ConstantProtected :
                image == CoCoImageKind.ConstPrivate ? KnownImageIds.ConstantPrivate :

                image == CoCoImageKind.DelegatePublic ? KnownImageIds.DelegatePublic :
                image == CoCoImageKind.DelegateInternal ? KnownImageIds.DelegateInternal :
                image == CoCoImageKind.DelegateProtected ? KnownImageIds.DelegateProtected :
                image == CoCoImageKind.DelegatePrivate ? KnownImageIds.DelegatePrivate :

                image == CoCoImageKind.EnumPublic ? KnownImageIds.EnumerationPublic :
                image == CoCoImageKind.EnumInternal ? KnownImageIds.EnumerationInternal :
                image == CoCoImageKind.EnumProtected ? KnownImageIds.EnumerationProtected :
                image == CoCoImageKind.EnumPrivate ? KnownImageIds.EnumerationPrivate :

                image == CoCoImageKind.EnumMemberPublic ? KnownImageIds.EnumerationItemPublic :
                image == CoCoImageKind.EnumMemberInternal ? KnownImageIds.EnumerationItemInternal :
                image == CoCoImageKind.EnumMemberProtected ? KnownImageIds.EnumerationItemProtected :
                image == CoCoImageKind.EnumMemberPrivate ? KnownImageIds.EnumerationItemPrivate :

                image == CoCoImageKind.EventPublic ? KnownImageIds.EventPublic :
                image == CoCoImageKind.EventInternal ? KnownImageIds.EventInternal :
                image == CoCoImageKind.EventProtected ? KnownImageIds.EventProtected :
                image == CoCoImageKind.EventPrivate ? KnownImageIds.EventPrivate :

                image == CoCoImageKind.ExtensionMethodPublic ? KnownImageIds.ExtensionMethod :
                image == CoCoImageKind.ExtensionMethodInternal ? KnownImageIds.ExtensionMethod :
                image == CoCoImageKind.ExtensionMethodProtected ? KnownImageIds.ExtensionMethod :
                image == CoCoImageKind.ExtensionMethodPrivate ? KnownImageIds.ExtensionMethod :

                image == CoCoImageKind.FieldPublic ? KnownImageIds.FieldPublic :
                image == CoCoImageKind.FieldInternal ? KnownImageIds.FieldInternal :
                image == CoCoImageKind.FieldProtected ? KnownImageIds.FieldProtected :
                image == CoCoImageKind.FieldPrivate ? KnownImageIds.FieldPrivate :

                image == CoCoImageKind.InterfacePublic ? KnownImageIds.InterfacePublic :
                image == CoCoImageKind.InterfaceInternal ? KnownImageIds.InterfaceInternal :
                image == CoCoImageKind.InterfaceProtected ? KnownImageIds.InterfaceProtected :
                image == CoCoImageKind.InterfacePrivate ? KnownImageIds.InterfacePrivate :

                image == CoCoImageKind.MethodPublic ? KnownImageIds.MethodPublic :
                image == CoCoImageKind.MethodInternal ? KnownImageIds.MethodInternal :
                image == CoCoImageKind.MethodProtected ? KnownImageIds.MethodProtected :
                image == CoCoImageKind.MethodPrivate ? KnownImageIds.MethodPrivate :

                image == CoCoImageKind.ModulePublic ? KnownImageIds.ModulePublic :
                image == CoCoImageKind.ModuleInternal ? KnownImageIds.ModuleInternal :
                image == CoCoImageKind.ModuleProtected ? KnownImageIds.ModuleProtected :
                image == CoCoImageKind.ModulePrivate ? KnownImageIds.ModulePrivate :

                image == CoCoImageKind.PropertyPublic ? KnownImageIds.PropertyPublic :
                image == CoCoImageKind.PropertyInternal ? KnownImageIds.PropertyPublic :
                image == CoCoImageKind.PropertyProtected ? KnownImageIds.PropertyProtected :
                image == CoCoImageKind.PropertyPrivate ? KnownImageIds.PropertyPrivate :

                image == CoCoImageKind.StructPublic ? KnownImageIds.ValueTypePublic :
                image == CoCoImageKind.StructInternal ? KnownImageIds.ValueTypeInternal :
                image == CoCoImageKind.StructProtected ? KnownImageIds.ValueTypeProtected :
                image == CoCoImageKind.StructPrivate ? KnownImageIds.ValueTypePrivate :

                image == CoCoImageKind.Label ? KnownImageIds.Label :
                image == CoCoImageKind.Local ? KnownImageIds.LocalVariable :
                image == CoCoImageKind.Namespace ? KnownImageIds.Namespace :
                image == CoCoImageKind.Parameter ? KnownImageIds.Parameter :
                image == CoCoImageKind.TypeParameter ? KnownImageIds.Type :
                image == CoCoImageKind.RangeVariable ? KnownImageIds.FieldPublic :
                image == CoCoImageKind.Error ? KnownImageIds.StatusError :
                -27;

            if (id == -27)
            {
                imageElement = default;
                return false;
            }

            imageElement = new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, id));
            return true;
        }
    }
}