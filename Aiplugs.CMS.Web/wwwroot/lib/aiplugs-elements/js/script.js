(function(global){
    if (typeof window.requestIdleCallback === 'function') {
        window.requestIdleCallback = function(callback) { return setTimeout(callback, 0); };
    }
    Stimulus.Application.prototype.resolve = function(element, identifier) {
        return this.getControllerForElementAndIdentifier(element, identifier);
    }
    Stimulus.Application.prototype.closestRoot = function(element, identifier) {
        const el = element.closest('[data-controller="'+identifier+'"]');
        return this.resolve(el, identifier);
    }
    Stimulus.Application.prototype.closestLeaf = function(element, identifier) {
        const el = element.querySelector('[data-controller="'+identifier+'"]');
        return this.resolve(el, identifier);
    }
    Stimulus.Application.prototype.find = function(query) {
        const el = document.querySelector(query);
        if (el) {
            for (let c of el.classList) {
                const ctl = this.resolve(el, c);
                if (ctl) return ctl;
            }
        }
        return null;
    }
    Stimulus.Controller.prototype.parent = function(identifier) {
        return this.application.closestRoot(this.element, identifier);
    }
    Stimulus.Controller.prototype.child = function(identifier) {
        return this.application.closestLeaf(this.element, identifier);
    }
    Stimulus.Controller.prototype.children = function(identifier) {
        return Array.from(this.element.querySelectorAll('[data-controller="'+identifier+'"]'))
                    .map(el => this.application.resolve(el, identifier));
    }
    Stimulus.Controller.prototype.debounce = function(code, dt, callback) {
        if (!this.__debounce) this.__debounce = {};
        if (this.__debounce[code]) clearTimeout(this.__debounce[code]);
        this.__debounce[code] = setTimeout(callback, dt);
    }
    Stimulus.Controller.prototype.throttle = function(code, dt, callback) {
        if (!this.__throttle) this.__throttle = {};
        if (!this.__throttle[code]) {
            requestIdleCallback(callback);
            this.__throttle[code] = setTimeout(() => {
                this.__throttle[code] = null;
            }, dt);
        }
    }
    Stimulus.Controller.prototype.initialized = function () {
        requestIdleCallback(() => {
            this.element.dispatchEvent(new CustomEvent('initialized'));
        });
    }
    Stimulus.Controller.prototype.disposable = function (callback) {
        const observer = new MutationObserver(records => {
            for (let record of records) {
                for (let node of record.removedNodes) {
                    const result = this.element.compareDocumentPosition(node);
                    if (result & this.element.DOCUMENT_POSITION_CONTAINS) {
                        callback();
                        observer.disconnect();
                        break;
                    }
                }
            }
        });
        observer.observe(document.body, { childList: true });
    }
    global.AiplugsElements =  Stimulus.Application.start();
}(window));

(function ($, aiplugs) {

    const registers = [];
    function register(method, url, handler) {
        registers.push({
            method,
            url,
            handler
        });
    }

    $.ajaxTransport('text', function (opts, settings) {
        let method = (settings.type || 'get').toLowerCase();
        const override = settings.headers['X-HTTP-Method-Override'];
        if (override) {
            method = override.toLowerCase();
        }
        for (let r of registers) {
            const m = r.url.exec(settings.url);
            if (method === r.method.toLowerCase() && m !== null) {
                return {
                    send: function (headers, completeCallback) {
                        completeCallback("200", "OK", { html: r.handler(m, settings.data) });
                    },
                    abort: function () {
                    }
                }
            }
        }
    });

    function decode(str) {
        return decodeURIComponent((str||'').replace(/\+/g, '%20'));
    }

    function parse(str) {
        return (str||'').split('&')
                        .map(set => set.split('='))
                        .reduce((a, b) => { a[decode(b[0])] = decode(b[1]); return a}, {});
    }

    register('PUT', /\/\/#(.+?)\/(.+)/, function (m, body) {
        const id = m[1];
        const prop = m[2];
        const el = document.getElementById(id);

        el[prop] = parse(body)[prop];
        el.dispatchEvent(new CustomEvent('change'));

        return '\n';
    })

    register('GET', /\/\/null/, function () {
        return '\n';
    })

    aiplugs.registerIcProxy = register;
    aiplugs.parseFormUrlEncoded = parse;
}(jQuery, AiplugsElements));

(function () {
    Intercooler.ready(function () {
        const observer = new MutationObserver(records => {
            for (let record of records) {
                for (let node of record.addedNodes) {
                    if (node.nodeType === node.ELEMENT_NODE && !node.hasAttribute('ic-id')) {
                        Intercooler.processNodes(node);
                    }
                }
            }
        });
        observer.observe(document.body, { childList: true });
    });
}());

(function(){
    $(function () {
        $('form').each(function (i, form) {
            const validation = $(form).data('validator');
            if (validation) {
                validation.settings.ignore = validation.settings.ignore + ', .val-ignore';
            }
        })

        const intersection = new IntersectionObserver((entries) => {
            for (const e of entries) {
                if (e.isIntersecting) {
                    $(e.target).trigger('in-viewport');
                }
            }
        });
        const mutation = new MutationObserver(records => {
            for (const record of records) {
                for (const node of record.addedNodes) {
                    if (node.nodeType === node.ELEMENT_NODE && node.classList.contains('in-viewport')) {
                        intersection.observe(node);
                    }
                }
            }
        });
        const elems = document.querySelectorAll('[ic-trigger-on="in-viewport"]');
        for (const elem of elems) {
            mutation.observe(elem.parentElement, { childList: true });
            intersection.observe(elem);
        }
    })
}())
AiplugsElements.register('aiplugs-dialog', class extends Stimulus.Controller {
    initialize() {
        this.initialized();
    }
    close() {
        this.element.remove();
    }
})
AiplugsElements.register("aiplugs-tinymce", class extends Stimulus.Controller {
  static get targets() {
    return ["textarea", "insertImage", "insertVideo"];
  }
  initialize() {
    const self = this;
    const defaultOptions = {
      selector: '#' + this.textareaTarget.id,
      plugins: ['paste', 'table', 'lists', 'code', 'link', 'save', 'template', 'image', 'media', 'anchor'],
      menubar: 'edit format insert',
      toolbar: [
        'undo redo | formatselect | removeformat bold italic underline | alignleft aligncenter alignright | outdent indent | bullist numlist | blockquote table fileimage filevideo insert | code',
      ],
      templates: [],
      paste_as_text: true,
      resize: false,
      height: document.body.scrollHeight - this.element.offsetTop - 110,
      save_onsavecallback: function () { },
      setup: function onSetup(editor) {
        editor.addButton('fileimage', {
          icon: 'image',
          tooltip: 'Insert image',
          onclick: function () {
            self.insertImageTarget.click();
          }
        });
        editor.addButton('filevideo', {
          icon: 'media',
          tooltip: 'Insert video',
          onclick: function () {
            self.insertVideoTarget.click();
          }
        });
      },
      init_instance_callback: function (editor) {
        editor.on('Change', function () {
          self.textareaTarget.value = editor.getContent();
        });
        self.disposable(() => {
          editor.remove();
        })
      }
    };
    Promise.all([
      this.getText(this.valueFrom),
      this.getText(this.settingsFrom),
    ]).then(values => {
      const value = values[0] || this.textareaTarget.value;
      const settings = JSON.parse(values[1] || '{}');
      const options = Object.assign(defaultOptions, settings);
      this.textareaTarget.value = value;
      tinymce.init(options);
    });
  }
  close() {
    tinymce.activeEditor.remove();
  }
  getText(url) {
    if (!url)
        return new Promise(resolve => { resolve(); });
    
    if (url.startsWith('#')) {
        const el = document.querySelector(url);
        return new Promise(resolve => { resolve(el.value || el.innerText); });
    }

    return fetch(url, {mode:'cors',credentials:'include'}).then(res => res.text());
  }
  get valueFrom() {
    return this.data.get('value-from') || '';
  }
  get settingsFrom() {
      return this.data.get('settings-from') || '';
  }
});

AiplugsElements.registerIcProxy('POST', /\/\/aiplugs-tinymce\/active\/images/, function (_, body) {
  const data = AiplugsElements.parseFormUrlEncoded(body);
  const src = data['src'];
  const alt = data['alt'];
  const title = data['title'];
  const html = `<img src="${src}" alt="${alt} title="${title}"/>`;
  tinymce.activeEditor.insertContent(html);
  return '\n';
})
AiplugsElements.registerIcProxy('POST', /\/\/aiplugs-tinymce\/active\/videos/, function (_, body) {
  const data = AiplugsElements.parseFormUrlEncoded(body);
  const src = data['src'];
  const alt = data['alt'];
  const title = data['title'];
  const html = `<video src="${src}" alt="${alt} title="${title}"></video>`;
  tinymce.activeEditor.insertContent(html);
  return '\n';
})
AiplugsElements.register('aiplugs-actions', class extends Stimulus.Controller {
    initialize() {
        this.update();
    }
    update() {
        const items = this.items;
        this.element.querySelectorAll('[when="any"]').forEach(el => {
            el.disabled = items <= 0;
            el.classList.toggle('--disabled', items <= 0);
        });
        this.element.querySelectorAll('[when="one"]').forEach(el => {
            el.disabled = items != 1;
            el.classList.toggle('--disabled', items != 1);
        });
    }
    get items() {
        return this.data.get('items');
    }
    set items(value) {
        this.data.set('items', value);
        this.update();
    }
})
AiplugsElements.register("aiplugs-list", class extends Stimulus.Controller {
  static get targets() {
    return ["item"];
  }
  select(e) {
    this.items.forEach(item => { item.unselect(); });
    this.application.closestRoot(e.target, "aiplugs-list-item").select();
  }
  dispatchUpdate() {
    this.throttle('change-selects', 100, () => {
      this.element.dispatchEvent(new CustomEvent('change'));
    })
  }
  get items() {
    return this.itemTargets.map(el => this.application.resolve(el, 'aiplugs-list-item'));
  }
  get selectedItem() {
    return this.items.filter(item => item.selected)[0];
  }
  get checkedItems() {
    return this.items.filter(item => item.checked);
  }
  get electedItems() {
    return this.items.filter(item => item.selected || item.checked);
  }
});

AiplugsElements.register("aiplugs-list-item", class extends Stimulus.Controller {
  static get targets() {
    return ["checkbox"];
  }
  initialize() {
    this.update();
  }
  update() {
    this.element.classList.toggle("--checked", this.checked);
    this.element.classList.toggle("--selected", this.selected);
    this.parent('aiplugs-list').dispatchUpdate();
  }
  select() {
    this.selected = true;
  }
  unselect() {
    this.selected = false;
  }
  get checked() {
    return this.checkboxTarget.checked;
  }
  set checked(value) {
    this.checkboxTarget.checked = value;
    this.update();
  }
  get selected() {
    return this.data.get('selected') === 'true';
  }
  set selected(value) {
    this.data.set('selected', value);
    this.update();
  }
});


AiplugsElements.register("aiplugs-checkbox", class extends Stimulus.Controller {
  static get targets() {
    return ["checkbox"];
  }
  initialize() {
    this.update();
  }
  update() {
    this.element.classList.toggle("aiplugs-checkbox--checked", this.checked);
  }
  get checked() {
    return this.checkboxTarget.checked;
  }
  set checked(value) {
    this.checkboxTarget.checked = value;
    this.update();
  }
    setNamePrefix(prefix) {
        this.checkboxTarget.name = prefix + '.' + (this.data.get('nameTemplate') || '');
    }
});
AiplugsElements.register("aiplugs-info", class extends Stimulus.Controller {
    static get targets() {
        return ['detail'];
    }
    update(){
        this.detailTarget.classList.toggle("--visible", this.visible);
    }
    toggle() {
        this.visible = !this.visible;
    }
    get visible() {
        return this.data.get('visible') === 'true';
    }
    set visible(value) {
        this.data.set('visible', value);
        this.update();
    }
});





AiplugsElements.register('aiplugs-monaco', class extends Stimulus.Controller {
    static get targets() {
        return ['progress', 'textarea'];
    }
    initialize() {
        require(['vs/editor/editor.main'], () => {
            this.init();
        });
    }
    init() {
        const created = monaco.editor.onDidCreateEditor(() => {
            this.progressTarget.remove();
            created.dispose();
        });
        Promise.all([
            this.getText(this.valueFrom),
            this.getText(this.settingsFrom),
        ]).then(values => {
            const value = values[0] || this.textareaTarget.value;
            const settings = JSON.parse(values[1] || '{}');
            const options = Object.assign({ language: 'html' }, settings, { value: value } );
            const editor = monaco.editor.create(this.element, options);
            this.textareaTarget.value = value;
            editor.getModel().onDidChangeContent(() => {
                this.textareaTarget.value = editor.getValue();
            })
            editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KEY_S, function() {
                // prevent browser action;
            });
            window.addEventListener('resize', () => {
                this.throttle('resize', 500, () => {
                    editor.layout();
                })
            });
            new ResizeObserver(() => {
                this.throttle('resize', 500, () => {
                    editor.layout();
                })
            }).observe(this.element);
            this.disposable(() => {
                editor.dispose();
            })
            const current = monaco.languages.json.jsonDefaults.diagnosticsOptions.schemas;
            const additions = this.jsonSchemas.filter(uri => !current.some(schema => schema.uri === uri));
            for (let uri of additions) {
                fetch(uri)
                    .then(res => res.json())
                    .then(schema => {
                        const schemas = monaco.languages.json.jsonDefaults.diagnosticsOptions.schemas.concat([{ uri, schema }]);
                        monaco.languages.json.jsonDefaults.setDiagnosticsOptions({ validate: true, schemas });
                    })
            }
        })
    }
    getText(url) {
        if (!url)
            return new Promise(resolve => { resolve(); });
        
        if (url.startsWith('#')) {
            const el = document.querySelector(url);
            return new Promise(resolve => { resolve(el.value || el.innerText); });
        }

        return fetch(url, {mode:'cors',credentials:'include'}).then(res => res.text());
    }
    get valueFrom() {
        return this.data.get('value-from') || '';
    }
    get settingsFrom() {
        return this.data.get('settings-from') || '';
    }
    get jsonSchemas() {
        return (this.data.get('json-schemas') || '').split(',').filter(uri => !!uri);
    }
});
AiplugsElements.register("aiplugs-dictionary", class extends Stimulus.Controller {
    static get targets() {
        return ["items", "template", "message", "item"];
    }
    initialize() {
        this.element.closest("form").addEventListener("submit", e => {
            if (!this.validate()) {
                e.preventDefault();
                return false;
            }
        });
    }
    add() {
        const content = this.templateTarget.content.cloneNode(true);
        this.itemsTarget.appendChild(content);
        setTimeout(() => {
            const item = this.items.pop();
            item.itemKeyTarget.focus();
            item.name = this.name;
        }, 0);
    }
    update() {
        this.items.forEach(item => {
            item.update();
        });
    }
    validate() {
        const messages = [];
        const mk = this.regexKey;
        const mv = this.regexValue;
        const rk = this.regexKeyPattern;
        const rv = this.regexValuePattern;
        const items = this.items;

        items.forEach(item => {
            const mk = !rk.exec(item.key) ? this.regexKey : null;
            const mv = !rv.exec(item.value) ? this.regexValue : null;
            item.validKey = !mk;
            item.validValue = !mv;
            messages.push(mk);
            messages.push(mv);
        });
        
        for (let i = 0; i < items.length; i++) {
            for (let j = 0; j < i; j++) {
                if (items[i].key === items[j].key) {
                    items[j].validKey = false;
                    items[i].validKey = false;
                    messages.push(this.duplicateKey);
                }
            }
        }

        this.message = messages.filter((_, i) => _ !== null && messages.indexOf(_) === i).join("<br>");

        return !this.message;
    }
    get items() {
        return this.itemTargets.map(item => this.application.resolve(item, "aiplugs-dictionary-item"));
    }
    set message(value) {
        this.messageTarget.innerHTML = value;
    }
    get regexKey() {
        return this.data.get("regex-key");
    }
    get regexKeyPattern() {
        return new RegExp(this.data.get("regex-key-pattern") || "^.*$");
    }
    get regexValue() {
        return this.data.get("regex-value");
    }
    get regexValuePattern() {
        return new RegExp(this.data.get("regex-value-pattern") || "^.*$");
    }
    get duplicateKey() {
        return this.data.get("duplicate-key") || "Duplicate keys detected.";
    }
    get name() {
        return this.data.get('name') || '';
    }
    set name(value) {
        this.data.set('name', value);
    }
    setNamePrefix(prefix) {
        const name = prefix + '.' + (this.data.get('nameTemplate') || '');
        for (let item of this.children('aiplugs-dictionary-item')) {
            item.name = name;
        }
        this.name = name;
    }
});
AiplugsElements.register("aiplugs-dictionary-item", class extends Stimulus.Controller {
    static get targets() {
        return ["itemKey", "itemValue"];
    }
    update() {
        const name = this.name;
        if (name !== null) {
            this.itemValueTarget.name = name;
        }
    }
    remove() {
        const dict = this.application.closestRoot(this.element, "aiplugs-dictionary");
        this.element.remove();
        dict.validate();
    }
    get name() {
        const n = this.data.get("name") || "";
        const k = this.itemKeyTarget.value;
        return k ? `${n}[${k}]` : null;
    }
    set name(value) {
        this.data.set('name', value);
        this.update();
    }
    get key() {
        return this.itemKeyTarget.value;
    }
    get value() {
        return this.itemValueTarget.value;
    }
    set validKey(value) {
        this.itemKeyTarget.classList.toggle("aiplugs-dictionary__item--invalid", !value);
    }
    set validValue(value) {
        this.itemValueTarget.classList.toggle("aiplugs-dictionary__item--invalid", !value);
    }
});
AiplugsElements.register('aiplugs-modal', class extends Stimulus.Controller {
    close() {
        this.element.remove();
    }
});
AiplugsElements.register('aiplugs-nav', class extends Stimulus.Controller {
    initialize() {
        this.force = this.fold;
        window.addEventListener('resize', () => {
            this.resize();
        })
        this.update();
        this.resize();
    }
    toggle() {
        this.fold = !this.fold;
        this.force = !this.force;
        this.update();
    }
    resize() {
        if (!this.force) {   
            this.fold = window.innerWidth < this.threshold;
            this.update();
        }
    }
    update() {
        this.element.classList.toggle('--fold', this.fold);
        this.element.classList.toggle('--force', this.force);
    }
    get fold() {
        return sessionStorage.getItem('aiplugs-nav-fold') === 'true';
    }
    set fold(value) {
        sessionStorage.setItem('aiplugs-nav-fold', value);
    }
    get force() {
        return sessionStorage.getItem('aiplugs-nav-force') === 'true';
    }
    set force(value) {
        sessionStorage.setItem('aiplugs-nav-force', value);
    }
    get threshold() {
        return parseInt(this.data.get('threshold') || 800);
    }
})
AiplugsElements.register('aiplugs-blade', class extends Stimulus.Controller {
    initialize() {
        this.update();
    }
    close() {
        this.element.remove();
    }
    toggle() {
        this.expanded = !this.expanded;
    }
    update() {
        this.element.classList.toggle('--expanded', this.expanded);
    }
    get expanded() {
        return this.data.get('expanded') === 'true';
    }
    set expanded(value) {
        this.data.set('expanded', value);
        this.update();
    }
});
AiplugsElements.register("aiplugs-code", class extends Stimulus.Controller {
    static get targets() {
        return ["view","input"];
    }
    initialize() {
        this.inputTarget.addEventListener('change', () => {
            this._setValue(this.value);
        })
    }
    edit() {
        const target = this.container;
        const template = this.element.querySelector("template:not([data-controller='aiplugs-dialog-template'])");
        const id = this.editorId || (this.editorId = "editor-" + (this.inputTarget.id || (~~(Math.random() * Math.pow(2, 16))).toString(16)));
        const blade = target.querySelector(`#${id}`);
        
        if (!blade && template) {
            const content = template.content.cloneNode(true);
            content.firstElementChild.setAttribute('id', id);
            target.appendChild(content);
        }
    }
    update() {
        this.element.classList.toggle("aiplugs-code--visible-all", this.visibleView);
    }
    toggleView() {
        this.visibleView = !this.visibleView;
    }
    _setValue(value) {
        this.viewTarget.innerText = value;
        hljs.highlightBlock(this.viewTarget);
    }
    get value() {
        return this.inputTarget.value;
    }
    set value(value) {
        this.inputTarget.value = value;
        this._setValue(value);
    }
    get visibleView() {
        return this.data.get("visible-view") === "true";
    }
    set visibleView(value) {
        this.data.set("visible-view", value);
        this.update();
    }
    get container() {
        return document.querySelector(this.data.get("container")) || document.body;
    }
    setNamePrefix(prefix) {
        this.inputTarget.name = prefix + '.' + (this.data.get('nameTemplate') || '');
    }
});
AiplugsElements.register("aiplugs-input", class extends Stimulus.Controller {
  static get targets() {
    return ["input", "suggestion"];
  }
  initialize() {
    if (this.valueFrom) {
      const el = document.querySelector(this.valueFrom);
      if (el) {
        this.inputTarget.value = el.value;
      }
    }
  }
  search() {
    const url = this.ajaxUrl.replace("{0}", this.inputTarget.value);
    const opts = {
      method: "GET",
      headers: this.ajaxHeaders,
      mode: "cors",
      credentials: "include"
    }
    return fetch(url, opts).then(res => {
      if (res.ok)
        return res.json();
    }).then(data => Array.isArray(data) ? data : []);
  }
  onInput() {
    this.debounce('oninput', 300, () => {
      if (this.ajaxUrl) {
        if (this.unique)
          this.check();
        else
          this.suggestion();
      }
    });
  }
  onBlur() {
    if (this.ajaxUrl && !this.unique) {
      const value = this.inputTarget.value;
      const valueKey = this.ajaxValue;
      const rule = this.ignoreCase ? datum => (datum[valueKey] || "").toLowerCase() === value
        : datum => (datum[valueKey] || "") === value
      this.search().then(data => {
        const datum = data.find(rule) || data.find(datum => (datum[valueKey] || "").startsWith(value)) || data[0];
        if (datum) {
          this.inputTarget.value = datum[valueKey];
        }
      })
    }
  }
  check() {
    const value = this.inputTarget.value;
    const valueKey = this.ajaxValue;
    const rule = this.ignoreCase ? datum => (datum[valueKey] || "").toLowerCase() === value
      : datum => (datum[valueKey] || "") === value
    this.search().then(data => {
      const duplicated = !!data.find(rule);
      this.element.classList.toggle("aiplugs-input--duplicated", duplicated);
      this.element.classList.toggle("aiplugs-input--unique", !duplicated);
    })
  }
  suggestion() {
    const labelKey = this.ajaxLabel;
    const valueKey = this.ajaxValue;
    this.search().then(data => {
      this.suggestionTarget.innerHTML = "";
      data.forEach(datum => {
        const option = document.createElement("option");
        option.innerText = datum[labelKey];
        option.label = datum[labelKey];
        option.value = datum[valueKey];
        this.suggestionTarget.appendChild(option);
      });
    });
  }
  get unique() {
    return this.data.get("unique") === "true";
  }
  get ignoreCase() {
    return this.data.get("ignore-case") === "true";
  }
  get ajaxUrl() {
    return this.data.get("ajax-url");
  }
  get ajaxHeaders() {
    const prefix = "data-aiplugs-input-ajax-headers-";
    return Array.from(this.element.attributes)
      .filter(attr => attr.name.startsWith(prefix))
      .reduce((headers, attr) => {
        headers[attr.name.replace(prefix, "")] = attr.value;
      }, {});
  }
  get ajaxLabel() {
    return this.data.get("ajax-label") || "label";
  }
  get ajaxValue() {
    return this.data.get("ajax-value") || "value";
  }
  get valueFrom() {
    return this.data.get("value-from");
  }
    setNamePrefix(prefix) {
        this.inputTarget.name = prefix + '.' + (this.data.get('nameTemplate') || '');
    }
});
AiplugsElements.register("aiplugs-array", class extends Stimulus.Controller {
    static get targets() {
        return ["items", "add", "item"];
    }
    initialize() {
        this.update();
    }
    add() {
        const template = Array.from(this.itemsTarget.children).find(el => el.constructor === HTMLTemplateElement);
        if (template) {
            const content = template.content.cloneNode(true);
            this.itemsTarget.appendChild(content); 
            this.update();
        }
    }
    update() {
        setTimeout(() => {
            this.items.forEach((item, index) => {
                item.index = index;

                const nameTemplate = this.nameTemplate;
                const prefix = nameTemplate.replace("{0}", index);//.replace(/(?<!\{)\{0\}(?!\})/, index).replace('{{', '{').replace('}}', '}');
                const elements = ["aiplugs-input", "aiplugs-textarea", "aiplugs-code", "aiplugs-tag", "aiplugs-select", "aiplugs-checkbox", "aiplugs-dictionary", "aiplugs-array"];
                
                for (let elemName of elements) {
                    for (let elem of item.children(elemName)) {
                        elem.setNamePrefix(prefix);
                    }
                }
            });
        }, 0);
    }
    get items () {
        return this.itemTargets.map(el => this.application.getControllerForElementAndIdentifier(el, "aiplugs-array-item")).filter(_ => _);
    }
    get nameTemplate() {
        return this.data.get('name') || '';
    }
    set nameTemplate(value) {
        this.data.set('name', value);
        this.update();
    }
    setNamePrefix(prefix) {
        this.nameTemplate = prefix + '.' + (this.data.get('nameTemplate') || '');
    }
});
AiplugsElements.register("aiplugs-array-item", class extends Stimulus.Controller {
    static get targets() {
        return ["label", "up", "down", "remove", "label"];
    }
    initialize() {
        this.update();
    }
    up() {
        if (!this.upDisabled) {
            const target = this.element.previousElementSibling;
            target.insertAdjacentElement('beforebegin', this.element);
            this.update();
            this.application.getControllerForElementAndIdentifier(target, "aiplugs-array-item").update();
            this.application.getControllerForElementAndIdentifier(this.element.closest("[data-controller='aiplugs-array']"), "aiplugs-array").update();
        }
    }
    down() {
        if (!this.downDisabled) {
            const target = this.element.nextElementSibling;
            target.insertAdjacentElement('afterend', this.element);
            this.update();
            this.application.getControllerForElementAndIdentifier(target, "aiplugs-array-item").update();
            this.application.getControllerForElementAndIdentifier(this.element.closest("[data-controller='aiplugs-array']"), "aiplugs-array").update();                
        }
    }
    remove() {
        const next = !this.downDisabled ? this.element.nextElementSibling : null;
        
        this.element.remove();

        if (next) {
            this.application.getControllerForElementAndIdentifier(next, "aiplugs-array-item").update();
            this.application.getControllerForElementAndIdentifier(next.closest("[data-controller='aiplugs-array']"), "aiplugs-array").update();
        }
    }
    update() {
        this.upTarget.disabled = this.upDisabled;
        this.downTarget.disabled = this.downDisabled;
        this.removeTarget.disabled = this.removeDisabled;
        this.labelTarget.innerText = this.label + "#" + this.index;
        
    }
    get upDisabled() {
        const el = this.element.previousElementSibling;
        return !el || el.constructor === HTMLTemplateElement;
    }
    get downDisabled() {
        const el = this.element.nextElementSibling;
        return !el || el.constructor === HTMLTemplateElement;
    }
    get removeDisabled() {
        return false;
    }
    get label() {
        return this.data.get("label") || "";
    }
    set label(value) {
        this.data.set("label", value);
        this.update();
    }
    get index() {
        return  (parseInt(this.data.get("index")) || 0) + 1;
    }
    set index(value) {
        this.data.set("index", value);
        this.update();
    }
});
AiplugsElements.register("aiplugs-select", class extends Stimulus.Controller {
    static get targets() {
        return ["checkbox"];
    }
    initialize() {
        this.update();
    }
    update() {
        this.checkboxTargets.forEach(el => {
            el.parentElement.classList.toggle("aiplugs-select__checkbox--checked", el.checked);
        });
    }
    setNamePrefix(prefix) {
        for (let input of this.checkboxTargets) {
            input.name = prefix + '.' + (this.data.get('nameTemplate') || '') + (input.name.endsWith('[]') ? '[]' : '');
        }
    }
});
AiplugsElements.register("aiplugs-tag", class extends Stimulus.Controller {
    static get targets() {
        return ["template","input","suggestion","item"];
    }
    update() {
        this.element.classList.toggle("aiplugs-tag--focus-last", this.focusLast);
    }
    onKeydown(e) {
        if (this.debounceId)
            clearTimeout(this.debounceId);

        this.debounceId = setTimeout(() => {
            if (this.ajaxUrl)
                this.suggestion();
        }, 100);

        if (e.keyCode === 13) { // ENTER
            e.preventDefault();
            this.add();
        }
        else if (e.keyCode === 8) { // BACKSPACE
            this.tryRemoveLast();
        }
        else if (e.keyCode === 39 || e.keyCode == 27) { // RIGHT || ESCAPE
            this.tryCancelRemoveLast();
        }
    }
    exists(value) {
        if (this.ignoreCase) 
            return !!this.items.find(item => item.value.toLowerCase() === value.toLowerCase());
        
        return !!this.items.find(item => item.value === value)
    }
    add() {
        if (this.inputTarget.value.length == 0)
            return; 

        const value = this.inputTarget.value;
        const option = this.ajaxUrl ? this.suggestionTarget.querySelector(`option[value="${value}"]`) : null;
        const label = option ? option.label : value;

        if (this.ajaxUrl && !option)
            return;
        
        if (!this.exists(value)) {
            const content = this.templateTarget.content.cloneNode(true);
            this.inputTarget.parentElement.insertBefore(content, this.inputTarget);
            setTimeout(() => {
                const item = this.items.pop();
                item.label = label;
                item.value = value;
                item.name = this.name;
                item.validate();
                this.inputTarget.value = "";
            }, 0);
        }
    }
    tryRemoveLast() {
        if (this.inputTarget.value.length === 0 && this.items.length > 0) {
            if (this.focusLast) {
              this.items.pop().remove();
              this.focusLast = false;
            }
            else {
              this.focusLast = true;
            }
          }
    }
    tryCancelRemoveLast() {
        if (this.focusLast) {
            this.focusLast = false;
        }
    }
    search() {
        const url = this.ajaxUrl.replace("{0}", this.inputTarget.value);
        const opts = {
            method: "GET",
            headers: this.ajaxHeaders,
            mode: "cors",
            credentials: "include"
        }
        return fetch(url, opts).then(res => {
            if (res.ok)
                return res.json();
        }).then(data => Array.isArray(data) ? data : []);
    }
    suggestion() {
        const labelKey = this.ajaxLabel;
        const valueKey = this.ajaxValue;
        this.search().then(data => {
            this.suggestionTarget.innerHTML = "";
            data.forEach(datum => {
                const option = document.createElement("option");
                option.label = datum[labelKey];
                option.value = datum[valueKey];
                this.suggestionTarget.appendChild(option);
            });
        });
    }
    get items() {
        return this.itemTargets.map(item => this.application.resolve(item, "aiplugs-tag-item"));
    }
    get focusLast() {
        return this.data.get("focus-last") === "true";
    }
    set focusLast(value) {
        this.data.set("focus-last", value);
        this.update();
    }
    get ignoreCase() {
        return this.data.get("ignore-case") === "true";
    }
    get ajaxUrl() {
        return this.data.get("ajax-url");
    }
    get ajaxHeaders() {
        const prefix = "data-aiplugs-tag-ajax-headers-";
        return Array.from(this.element.attributes)
            .filter(attr => attr.name.startsWith(prefix))
            .reduce((headers, attr) => {
                headers[attr.name.replace(prefix,"")] = attr.value;
            }, {});
    }
    get ajaxLabel() {
        return this.data.get("ajax-label") || "label";
    }
    get ajaxValue() {
        return this.data.get("ajax-value") || "value";
    }
    get name() {
        return this.data.get('name') || '';
    }
    set name(value) {
        this.data.set('name', value);
    }
    setNamePrefix(prefix) {
        const name = prefix + '.' + (this.data.get('nameTemplate') || '');
        for (let item of this.children('aiplugs-tag-item')) {
            item.name = name;
        }
        this.name = name;
    }
});
AiplugsElements.register("aiplugs-tag-item", class extends Stimulus.Controller {
    static get targets() {
        return ["input", "label"];
    }
    initialize() {
        //$.validator.unobtrusive.parseElement(this.inputTarget, false);
    }
    validate(form, name) {
        form = form || this.element.closest("form");
        name = name || this.name;
        $(form).validate().element(`[name="${name}"]`);
    }
    remove() {
        const form = this.element.closest("form");
        const name = this.name;
        this.element.remove();
        this.validate(form, name);
    }
    get name() {
        return this.inputTarget.name;
    }
    set name(value) {
        this.inputTarget.name = value;
    }
    get value() {
        return this.inputTarget.value;
    }
    set value(value) {
        this.inputTarget.value = value;
    }
    get label() {
        return this.labelTarget.innerText;
    }
    set label(value) {
        this.labelTarget.innerText = value;
    }
});
AiplugsElements.register("aiplugs-textarea", class extends Stimulus.Controller {
    static get targets() {
        return ["textarea"];
    }
    connect() {
        const input = this.textareaTarget;
        const style = getComputedStyle(input);
        this._diffWidth = parseInt(style.paddingLeft) + parseInt(style.paddingRight);
        this._diffHeight = -(parseInt(style.paddingBottom) + parseInt(style.paddingTop));
        const next = window.requestIdleCallback || (callback => { setTimeout(callback, 100); });
        next(() => {
          this.updateHeight();
        })
    }
    updateHeight() {
        const container = this.element;
        const input = this.textareaTarget;
        const len = (input.value || '').length;
        const style = getComputedStyle(input);
        const scrollTop = container.scrollTop;

        if (this._len >= len) {
          input.style.height = 'auto';
        }

        if (input.scrollHeight > input.clientHeight) {
          input.style.height = input.scrollHeight + this._diffHeight + 'px';
        }

        this._len = len;
        container.scrollTop = scrollTop;
    }
    setNamePrefix(prefix) {
        this.textareaTarget.name = prefix + '.' + (this.data.get('nameTemplate') || '');
    }
});