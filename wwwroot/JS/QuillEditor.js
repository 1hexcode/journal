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
