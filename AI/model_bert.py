from transformers import AutoTokenizer, TFAutoModel
import tensorflow as tf
from tensorflow.keras.layers import Layer

bert_name = "vinai/phobert-base"
bert = TFAutoModel.from_pretrained(bert_name)
tokenizer = AutoTokenizer.from_pretrained(bert_name)

class BERTEmbedding:
    def __init__(self, tokenizer, model):
        # Load BERT tokenizer and model
        self.tokenizer = tokenizer
        self.model = model
        
    def encode(self, text):
        # Tokenize input and create embeddings
        inputs = self.tokenizer(text, return_tensors="tf", padding=True, truncation=True)

        # Get the model outputs
        outputs = self.model(**inputs)

        embeddings = tf.reduce_mean(outputs.last_hidden_state, axis=1)
        return embeddings
    
bert_embedding = BERTEmbedding(model = bert, tokenizer = tokenizer)

class BERTEmbeddingLayer(Layer):
    def __init__(self, bert_model_name, max_length, **kwargs):
        super(BERTEmbeddingLayer, self).__init__(**kwargs)
        self.bert_model_name = bert_model_name
        self.max_length = max_length
        # Load the BERT model inside the layer
        self.bert = TFAutoModel.from_pretrained(bert_model_name)

    def call(self, inputs):
        input_ids, attention_mask = inputs
        # Obtain BERT outputs
        outputs = self.bert(input_ids=input_ids, attention_mask=attention_mask)
        return outputs.last_hidden_state  # Equivalent to outputs[0]

    def get_config(self):
        config = super(BERTEmbeddingLayer, self).get_config()
        config.update({
            'bert_model_name': self.bert_model_name,
            'max_length': self.max_length,
        })
        return config

    @classmethod
    def from_config(cls, config):
        return cls(**config)