import tensorflow as tf
from transformers import AutoTokenizer, TFAutoModel
from tensorflow.keras.layers import Input, Lambda, Dense, Concatenate, Conv1D, GlobalAveragePooling1D, Dropout, BatchNormalization, Bidirectional, LSTM, Layer
from tensorflow.keras.models import Model
from tensorflow.keras.callbacks import EarlyStopping, ReduceLROnPlateau
from sklearn.utils import class_weight
import numpy as np
from model_bert import bert, tokenizer, BERTEmbeddingLayer

model = tf.keras.models.load_model('model_coffee_question_classification.keras', custom_objects={'BERTEmbeddingLayer': BERTEmbeddingLayer})

MAX_LEN_CONTEXT = 25

def getNumberPredict(question):
  test_contexts = [question]

  # Tokenize the test contexts
  test_encodings = tokenizer(
      test_contexts,
      truncation=True,
      padding='max_length',
      max_length=MAX_LEN_CONTEXT,
      add_special_tokens=True,
      return_tensors='tf'
  )

  # Prepare input tensors
  test_input_ids = test_encodings['input_ids']
  test_attention_mask = test_encodings['attention_mask']

  # Make predictions using the model
  predictions = model.predict([test_input_ids, test_attention_mask], batch_size=32)

  # Convert predictions to class labels (e.g., using argmax)
  predicted_classes = np.argmax(predictions, axis=1)

  return predicted_classes[0]

  # If you have a dictionary mapping from class indices to class labels, apply it
  # Example:
  label_mapping = {0: 'infomation', 1: 'productTopSell', 2: 'recommendedProduct', 3: 'ingredient', 4: 'quality', 5: 'greeting'}
  predicted_labels = [label_mapping.get(pred, 'unknown') for pred in predicted_classes]

  # Print the predicted labels
  for context, label in zip(test_contexts, predicted_labels):
      print(f"Question: {context} --> Predicted Label: {label}")

print(getNumberPredict("cà phê nào ngon"))