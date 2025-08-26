import React from 'react';
import {
  View,
  Text,
  StyleSheet,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';

const OrdersScreen: React.FC = () => {
  return (
    <View style={styles.container}>
      <View style={styles.iconContainer}>
        <Ionicons name="receipt-outline" size={64} color="#667eea" />
      </View>
      <Text style={styles.title}>Sale Orders</Text>
      <Text style={styles.subtitle}>Order management functionality coming soon...</Text>
      <Text style={styles.description}>
        This screen will show order management features including:
        {'\n'}• View all sale orders
        {'\n'}• Create new orders
        {'\n'}• Track order status
        {'\n'}• Process payments
        {'\n'}• Generate invoices
      </Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f8f9fa',
    padding: 20,
  },
  iconContainer: {
    marginBottom: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 10,
  },
  subtitle: {
    fontSize: 16,
    color: '#666',
    textAlign: 'center',
    marginBottom: 20,
  },
  description: {
    fontSize: 14,
    color: '#666',
    textAlign: 'center',
    lineHeight: 22,
  },
});

export default OrdersScreen;