define([
        "dojo/_base/declare",
        "dojo/_base/lang",
        "dojo/_base/connect",
        "dojo/when",
        "dojo/dnd/Source",
        "dojo/dom-construct",
        "dojo/dom-class",
        "dijit/registry",
        "dijit/_CssStateMixin",
        "dijit/_Widget",
        "dijit/_TemplatedMixin",
        "dijit/_WidgetsInTemplateMixin",
        "dijit/form/Textarea",
        "epi/epi",
        "epi/shell/widget/_ValueRequiredMixin",
        "epi/shell/dnd/Target",
        "epi/shell/widget/WidgetFactory",
        "epi/shell/MetadataTransformer",
        "./CollectionItem",
        "dojo/text!./content/CollectionTemplate.html",
        "xstyle/css!./content/styles.css"
    ],
    function(
        declare,
        lang,
        connect,
        when,
        DndSource,
        domConstruct,
        domClass,
        registry,
        _CssStateMixin,
        _Widget,
        _TemplatedMixin,
        _WidgetsInTemplateMixin,
        Textarea,
        epi,
        _ValueRequiredMixin,
        Target,
        WidgetFactory,
        MetadataTransformer,
        CollectionItem,
        template
    ) {

        return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin],
        {
            templateString: template,

            baseClass: "valueList",

            acceptDataTypes: ["listItem"],

            postCreate: function() {
                this.inherited(arguments);

                this.connect(this.addLink, "click", lang.hitch(this, this._onItemAdd));
                var metadataTransformer = new MetadataTransformer();
                var innerPropertySettings = this.metadata.customEditorSettings.innerPropertySettings;

                innerPropertySettings.name = this.metadata.name;

                var definitions = metadataTransformer.toComponentDefinitions({
                        properties: [innerPropertySettings]
                    },
                    "",
                    false,
                    false);

                this.innerPropertyComponentDefinition = definitions[0].components[0];


                // drag items support
                this.dndSource = new DndSource(this.itemsContainer,
                {
                    creator: lang.hitch(this, this._createDndElement),
                    copyOnly: true,
                    selfAccept: false,
                    selfCopy: false,
                    accept: this.acceptDataTypes
                });

            },

            _createDndElement: function(item, hint) {
                var value = "";
                var widget = registry.byId(item);
                if (widget) {
                    value = widget.getValue();
                }

                var dndTypes = this.acceptDataTypes,
                    node = domConstruct.create("div", { "class": "TEST" }).appendChild(document.createTextNode(value));

                return {
                    "node": node,
                    "type": dndTypes,
                    "data": item
                };
            },

            _onItemAdd: function() {
                this._createItem();
                this._set("value", this._getValue());
            },

            _createItem: function(value) {
                var listItemWidget = new CollectionItem();
                listItemWidget.placeAt(this.itemsContainer);
                listItemWidget.set("readOnly", this.get("readOnly"));

                var widgetFactory = new WidgetFactory();
                var handler = widgetFactory.getHandler(this);

                var def = handler.instantiateWidgetFunction(this.innerPropertyComponentDefinition);
                when(def,
                    lang.hitch(this,
                        function(widgetInstance) {
                            listItemWidget.setEditor(widgetInstance);
                            if (value) {
                                listItemWidget.set("value", value);
                            }
                            this.connect(listItemWidget,
                                "onChange",
                                lang.hitch(this,
                                    function(value) {
                                        var valuesList = this._getValue();
                                        this._set("value", valuesList);
                                    }));

                            this.connect(listItemWidget,
                                "onDelete",
                                lang.hitch(this,
                                    function(id) {
                                        var widget = registry.byId(id);
                                        widget.destroyRecursive();
                                        var valuesList = this._getValue();
                                        this._set("value", valuesList);

                                    }));
                        }));

                this.dndSource.sync();

                this.connect(listItemWidget,
                    'onDropItem',
                    lang.hitch(this,
                        function(sourceId, targetId) {
                            if (this.readOnly) {
                                return;
                            }

                            var sourceWidget = registry.byId(sourceId);
                            var widgets = registry.findWidgets(this.itemsContainer);
                            for (var i = 0; i < widgets.length; i++) {
                                if (widgets[i].id === sourceId) {
                                    continue;
                                }

                                if (widgets[i].id === targetId) {
                                    sourceWidget.placeAt(this.itemsContainer, i + 1);
                                    break;
                                }
                            }

                            var valuesList = this._getValue();
                            this._set("value", valuesList);
                        }));
            },

            _getValue: function() {
                var result = [];

                var widgets = registry.findWidgets(this.itemsContainer);

                for (var i = 0; i < widgets.length; i++) {
                    result.push(widgets[i].getValue());
                }
                return result;
            },

            _setValueAttr: function(value) {
                if (!value) {
                    return;
                }

                for (var i = 0; i < value.length; i++) {
                    this._createItem(value[i]);
                }
            },

            _setReadOnlyAttr: function(value) {
                this.inherited(arguments);
                domClass.toggle(this.addLink, "hidden", value);

                var widgets = registry.findWidgets(this.itemsContainer);
                for (var i = 0; i < widgets.length; i++) {
                    widgets[i].set("readOnly", value);
                }
            }
    });
});