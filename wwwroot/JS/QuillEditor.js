let quill;

window.initQuill = (editorId) => {
    quill = new Quill(`#${editorId}`, {
        theme: "snow",
    });
};

window.clearQuill = function () {
    if (window.quill) {
        window.quill.setText('');
    }
};


window.getQuillHtml = () => {
    return quill.root.innerHTML;
};

window.setQuillHtml = (html) => {
    quill.root.innerHTML = html;
};

window.quillInterop = {
    editor: null,

    initialize: function (elementId, dotNetRef) {
        const container = document.getElementById(elementId);
        if (!container) {
            console.error('Quill container not found:', elementId);
            return;
        }

        this.editor = new Quill('#' + elementId, {
            theme: 'snow',
            placeholder: 'Write your thoughts...',
            modules: {
                toolbar: [
                    [{ 'header': [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ 'color': [] }, { 'background': [] }],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    ['blockquote', 'code-block'],
                    ['link'],
                    ['clean']
                ]
            }
        });
    },

    getHtml: function () {
        if (!this.editor) return '';
        return this.editor.root.innerHTML;
    },

    getText: function () {
        if (!this.editor) return '';
        return this.editor.getText();
    },

    setHtml: function (html) {
        if (!this.editor) return;
        this.editor.root.innerHTML = html;
    },

    clear: function () {
        if (!this.editor) return;
        this.editor.setText('');
    }
};