define([
        //dojo
        "dojo/_base/declare",
        "dojo/_base/lang",
        "dojo/_base/connect",
        "dojo/on",
        "dojo/dom-class",
        //dijit
        "dijit/_CssStateMixin",
        "dijit/_Widget",
        "dijit/_TemplatedMixin",
        "dijit/_WidgetsInTemplateMixin",
        "dijit/form/Textarea",
        //epi
        "epi/epi",
        "epi/shell/dnd/Target",
        "dojo/text!./content/CollectionItemTemplate.html"
    ],
    function(
        //dojo
        declare,
        lang,
        connect,
        on,
        domClass,
        //dijit
        _CssStateMixin,
        _Widget,
        _TemplatedMixin,
        _WidgetsInTemplateMixin,
        Textarea,
        //epi
        epi,
        Target,
        template
    ) {
        return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin],
        {
            templateString: template,

            baseClass: "epiStringList",

            acceptDataTypes: ["listItem"],

            postCreate: function () {
                this.inherited(arguments);

                dojo.attr(this.domNode, { dndData: this.id });

                this._dropTarget = new Target(this.dropAreaNode, {
                    accept: this.acceptDataTypes,
                    createItemOnDrop: false,
                    readOnly: this.readOnly
                });

                this.connect(this._dropTarget, "onDropData", "_onDropData");
                this.connect(this._dropTarget, "onDndStart", "_onDragStart");
                this.connect(this._dropTarget, "onDndCancel", "_onDragCancel");

                on(this.deleteLink, "click", lang.hitch(this, function () {
                    this.onDelete(this.id);
                }));
            },

            //
            // Events
            //

            onChange: function(value) {
                // Event
            },

            onDelete: function(id) {
              // Event  
            },

            onDropItem: function (listItem, dndSourceItem) {
                // Event
            },


            setEditor: function(editorWidget) {
                this._editorWidget = editorWidget;
                editorWidget.placeAt(this.editorContainer);

                this.connect(editorWidget, "onChange",lang.hitch(this,
                    function(value) {
                        this.onChange(value);
                    }));
            },

            getValue: function() {
               // return this._editorWidget.get("value");
                return this._editorWidget.value;
            },

            _setValueAttr: function(value) {
                this._editorWidget.set("value", value);
            },

            _setReadOnlyAttr: function(value) {
                this.inherited(arguments);
                domClass.toggle(this.deleteLink, "hidden", value);
            },

            //
            // D&D
            //

            _onDropData: function(dndData, source, nodes, copy) {
                var dropItem = dndData ? (dndData.length ? dndData[0] : dndData) : null;

                if (dropItem) {
                    dojo.when(dropItem.data,
                        lang.hitch(this,
                            function(value) {
                                this.onDropItem(value, this.id);
                            }));
                }
                this._onDragEnd();
            },

            _onDragStart: function(source, nodes, copy) {
                if (this.readOnly) {
                    return;
                }

                if (nodes.length === 1) {
                    // cannot drop on source
                    if (nodes[0] == this.domNode) {
                        return;
                    }

                    var dndData = source.getItem(nodes[0].id);
                    if (dndData && dndData.type && dndData.type.length > 0) {
                        if (dndData.type[0] !== "listItem") {
                            return;
                        }
                    }

                    dojo.addClass(this.dropAreaNode, 'allow-drop');
                }
            },

            _onDragCancel: function() {
                this._onDragEnd();
            },

            _onDragEnd: function() {
                dojo.removeClass(this.dropAreaNode, 'allow-drop');
            }
    });
});